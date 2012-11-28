using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Federation.Core
{
    public class VotingServiceImpl : IVotingService
    {
        #region Petitions

        public Petition CreatePetition(PetitionContainer data, Guid userId, bool saveChanges)
        {
            if (data.GroupId.HasValue)
            {
                var uig = GroupService.UserInGroup(userId, data.GroupId.Value);
                if (uig == null)
                    throw new BusinessLogicException("Только члены могут создавать петиции в группе");

                if (uig.State == (byte)GroupMemberState.NotApproved)
                    throw new BusinessLogicException("Вы ожидаете одобрения модератора");
            }

            var petition = new Petition
            {
                AuthorId = userId,
                CreationDate = DateTime.Now,
                Duration = 365,
                GroupId = data.GroupId.HasValue ? data.GroupId.Value : (Guid?)null,
                IsDiscussionClosed = false,
                IsPrivate = data.IsPrivate,
                Signers = new List<User>(),
                State = (byte)ContentState.Draft,
                Tags = TagsHelper.ConvertStringToTagList(data.Tags, data.GroupId),
                Text = data.Text,
                Title = data.Title
            };

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            petition.Signers.Add(user);

            DataService.PerThread.ContentSet.AddObject(petition);

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            return petition;
        }

        public Petition EditPetition(PetitionContainer data, Guid userId, bool saveChanges)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == data.Id);
            if (petition == null)
                throw new BusinessLogicException("Не найдена петиция с указанным идентификатором");

            if (petition.State != (byte)ContentState.Draft)
                throw new BusinessLogicException("Редактировать можно только черновые петиции");

            if (petition.GroupId.HasValue)
            {
                var uig = GroupService.UserInGroup(userId, petition.GroupId.Value);

                if (petition.AuthorId.HasValue)
                {
                    if (!(uig.State == (byte)GroupMemberState.Moderator || petition.AuthorId.Value == userId))
                        throw new BusinessLogicException("У вас нет прав редактировать данную петицию");
                }
                else if (uig.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("У вас нет прав редактировать данную петицию");
            }
            else if (petition.AuthorId != userId)
                throw new BusinessLogicException("Вы не являетесть автором данной петиции");

            petition.Title = data.Title;
            petition.Text = data.Text;
            petition.IsPrivate = data.IsPrivate;
            petition.Tags.Clear();

            foreach (var tag in TagsHelper.ConvertStringToTagList(data.Tags, petition.GroupId))
                petition.Tags.Add(tag);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            return petition;
        }

        public Petition SignPetition(Guid id, Guid userId, bool saveChanges)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Не найдена петиция с указанным идентификатором");

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            if (petition.Duration.HasValue && DateTime.Now > petition.CreationDate.AddDays(petition.Duration.Value))
                throw new BusinessLogicException("Сбор подписей уже завершен");

            if (petition.GroupId.HasValue && petition.IsPrivate)
            {
                var gm = GroupService.UserInGroup(user.Id, petition.GroupId.Value);
                if (gm == null)
                    throw new BusinessLogicException("Вы не состоите в группе, которой принадлежит данная петиция");

                if (gm.State == (byte)GroupMemberState.NotApproved)
                    throw new BusinessLogicException("Вы ожидаете одобрения модератора");
            }

            if (petition.Signers.Contains(user))
                throw new BusinessLogicException("Вы уже подписали данную петицию");

            petition.Signers.Add(user);

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            return petition;
        }

        public Petition PublishPetition(Guid id, Guid userId, bool saveChanges)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id && p.State == (byte)ContentState.Draft);
            if (petition == null)
                throw new BusinessLogicException("Не найден черновик с указанным идентификатором");

            if (petition.GroupId.HasValue)
            {
                var uig = GroupService.UserInGroup(userId, petition.GroupId.Value);

                if (petition.AuthorId.HasValue)
                {
                    if (uig.State != (byte)GroupMemberState.Moderator && petition.AuthorId.Value != userId)
                        throw new BusinessLogicException("У вас нет прав публиковать данную петицию");
                }
                else if (uig.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("У вас нет прав публиковать данную петицию");

                if (uig.State == (byte)GroupMemberState.Moderator)
                    petition.State = (byte)ContentState.Approved;
                else
                    petition.State = (byte)ContentState.Premoderated;
            }
            else
            {
                if (petition.AuthorId != userId)
                    throw new BusinessLogicException("Вы не являетесть автором данной петиции");

                petition.State = (byte)ContentState.Approved;
            }

            petition.PublishDate = DateTime.Now;

            IList<Coauthor> toDelete = new List<Coauthor>();
            foreach (var unactiveCoauthor in petition.Coauthors.Where(c => !c.IsAccepted.HasValue || !c.IsAccepted.Value).ToList())
            {
                petition.Coauthors.Remove(unactiveCoauthor);
                toDelete.Add(unactiveCoauthor);
            }
            foreach (var unactiveCoauthor in toDelete)
                DataService.PerThread.CoauthorSet.DeleteObject(unactiveCoauthor);

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            if (petition.Duration.HasValue)
                ScheduleService.AddJob(new FinishPetitionTask(petition.Id), petition.PublishDate.Value.AddDays(petition.Duration.Value), false, null, false);

            return petition;
        }

        public Coauthor InvitePetitionCoauthor(PetitionCoauthorContainer data, Guid userId, bool saveChanges)
        {
            if (string.IsNullOrWhiteSpace(data.UserName))
                throw new BusinessLogicException("Не указано ФИО пользователя");

            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == data.PetitionId);
            if (petition == null)
                throw new BusinessLogicException("Не найдена петиция с данным идентификатором");

            if (petition.State != (byte)ContentState.Draft)
                throw new BusinessLogicException("Приглашать соавторов можно только в еще неопубликованные петиции");
            if (petition.AuthorId != userId)
                throw new BusinessLogicException("Вы не являетесь автором данной петиции");

            var username = data.UserName.Trim().Split(' ');

            if (username.Count() != 3)
                throw new BusinessLogicException("Введены неверные данные");

            var surname = username[0];
            var firstname = username[1];
            var patronymic = username[2];

            var user = DataService.PerThread.BaseUserSet
                .OfType<User>().SingleOrDefault(u => u.SurName == surname && u.FirstName == firstname && u.Patronymic == patronymic);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            if (user.Id == userId)
                throw new BusinessLogicException("Вы уже являетесь автором петиции");

            if (petition.GroupId.HasValue && petition.IsPrivate)
            {
                var uig = GroupService.UserInGroup(user.Id, petition.GroupId.Value);
                if (uig == null)
                    throw new BusinessLogicException("Данная петиция только для членов группы, а указанный пользователь в группе не состоит");

                if (uig.State == (byte)GroupMemberState.NotApproved)
                    throw new BusinessLogicException("Данная петиция только для членов группы, а указанный пользователь еще не одобрен модераторами");
            }

            if (DataService.PerThread.CoauthorSet.Count(c => c.UserId == user.Id & c.PetitionId == data.PetitionId) > 0)
                throw new BusinessLogicException("Уже отправлено приглашение данному пользователю");

            var coauthor = new Coauthor
            {
                PetitionId = data.PetitionId,
                UserId = user.Id
            };

            DataService.PerThread.CoauthorSet.AddObject(coauthor);

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            var date = DateTime.Now;
            var msg = new MessageStruct
            {
                AuthorId = petition.AuthorId,
                RecipientId = coauthor.UserId,
                Text = MessageComposer.ComposePetitionNotice(coauthor.PetitionId, "Вы приглашены на соавторство в петиции" +
                    " <a href='" + UrlHelper.GetUrl("petitionnotices", "user", false) + "'>Ответить</a>."),
                Type = (byte)MessageType.PetitionNotice,
                Date = date
            };

            MessageService.Send(msg);

            return coauthor;
        }

        public Petition DeletePetitionCoauthor(Guid id, Guid userId, bool saveChanges)
        {
            var coauthor = DataService.PerThread.CoauthorSet.SingleOrDefault(x => x.Id == id);
            if (coauthor == null)
                throw new BusinessLogicException("Неверный идентификатор соавтора");

            var petition = coauthor.Petition;

            if (coauthor.Petition.State != (byte)ContentState.Draft)
                throw new BusinessLogicException("Удалять соавторов можно только из еще неопубликованных петиций");
            if (coauthor.Petition.AuthorId != userId)
                throw new BusinessLogicException("Вы не являетесь автором данной петиции");

            var msg = new MessageStruct();

            if (petition.AuthorId.HasValue)
                msg.AuthorId = petition.AuthorId.Value;

            if (!coauthor.IsAccepted.HasValue)
            {
                var date = DateTime.Now;
                msg.Date = date;
                msg.RecipientId = coauthor.UserId;
                msg.Text = MessageComposer.ComposePetitionNotice(coauthor.PetitionId, "Автор петиции удалил приглашение на соавторство.");
                msg.Type = (byte)MessageType.PetitionNotice;

                MessageService.Send(msg);
            }
            else if (coauthor.IsAccepted.Value)
            {
                var date = DateTime.Now;
                msg.Date = date;
                msg.RecipientId = coauthor.UserId;
                msg.Text = MessageComposer.ComposePetitionNotice(coauthor.PetitionId, "Автор петиции удалил вас из соавторов");
                msg.Type = (byte)MessageType.PetitionNotice;

                MessageService.Send(msg);
            }

            DataService.PerThread.CoauthorSet.DeleteObject(coauthor);

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            return petition;
        }

        public Petition RespondToPetitionInvite(Guid id, Guid userId, bool accept, bool saveChanges)
        {
            var coauthor = DataService.PerThread.CoauthorSet.SingleOrDefault(c => c.Id == id && c.UserId == userId);
            if (coauthor == null)
                throw new BusinessLogicException("Не найдено приглашения на ваше имя для данной петиции");

            var petition = coauthor.Petition;

            if (coauthor.Petition.State != (byte)ContentState.Draft)
                throw new BusinessLogicException("Вступать в соавторство можно только в еще неопубликованные петиции");

            if (coauthor.IsAccepted.HasValue)
            {
                if (coauthor.IsAccepted.Value)
                    throw new BusinessLogicException("Вы уже приняли приглашение на соавторство");

                throw new BusinessLogicException("Вы уже отказались быть соавторством");
            }

            coauthor.IsAccepted = accept;

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            if (accept)
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
                if (user == null)
                    throw new BusinessLogicException("Перезайдите");

                if (coauthor.Petition.GroupId.HasValue && coauthor.Petition.IsPrivate)
                {
                    var uig = GroupService.UserInGroup(user.Id, coauthor.Petition.GroupId.Value);
                    if (uig == null)
                        throw new BusinessLogicException("Данная петиция только для членов группы, и вы в ней не состоите");

                    if (uig.State == (byte)GroupMemberState.NotApproved)
                        throw new BusinessLogicException("Данная петиция только для членов группы, а вы еще ожидаете одобрения модераторов");
                }

                coauthor.Petition.Signers.Add(user);

                if (saveChanges)
                    DataService.PerThread.SaveChanges();

                if (coauthor.Petition.AuthorId.HasValue)
                {
                    var date = DateTime.Now;
                    MessageService.Send(new MessageStruct
                    {
                        Date = date,
                        AuthorId = coauthor.UserId,
                        RecipientId = coauthor.Petition.AuthorId.Value,
                        Text = MessageComposer.ComposePetitionNotice(coauthor.PetitionId, "<a href='" + UrlHelper.GetUrl<User>(coauthor.UserId) + "' target='_blank'>" + coauthor.User.FullName + "</a> " +
                            "принял приглашение на соавторство в петиции."),
                        Type = (byte)MessageType.PetitionNotice
                    });
                }
            }
            else
                if (coauthor.Petition.AuthorId.HasValue)
                {
                    var date = DateTime.Now;
                    MessageService.Send(new MessageStruct
                    {
                        Date = date,
                        AuthorId = coauthor.UserId,
                        RecipientId = coauthor.Petition.AuthorId.Value,
                        Text = MessageComposer.ComposePetitionNotice(coauthor.PetitionId, "<a href='" + UrlHelper.GetUrl<User>(coauthor.UserId) + "' target='_blank'>" + coauthor.User.FullName + "</a> " +
                            "отказался быть соавтором в петиции."),
                        Type = (byte)MessageType.PetitionNotice
                    });
                }

            return petition;
        }

        public void FinishPetition(Guid id)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(x => x.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Указан неверный идентификатор петиции");

            if (petition.State != (byte)ContentState.Approved || !petition.PublishDate.HasValue)
                throw new BusinessLogicException("Петиция еще не опубликована");
            if (petition.Duration.HasValue && petition.PublishDate.Value.AddDays(petition.Duration.Value) < DateTime.Now)
                throw new BusinessLogicException("Еще не подошло время");

            petition.IsFinished = true;
            DataService.PerThread.SaveChanges();
        }

        #endregion

        private delegate void AnalizeMissedBulletinsCallBack(Guid groupMemberId);

        private void AnalizeMissedBulletins(Guid groupMemberId)
        {
            var gm = DataService.PerThread.GroupMemberSet.SingleOrDefault(x => x.Id == groupMemberId);
            if (gm != null)
            {
                var polls = gm.Group.Content.OfType<Poll>().Where(x => x.Bulletins.Count(b => b.OwnerId == gm.Id) == 0);
                foreach (var poll in polls)
                {
                    AddBulletinRequest(poll.Id, gm.UserId);
                }
                var elections = gm.Group.Content.OfType<Election>().Where(x => x.ElectionBulletins.Count(b => b.OwnerId == gm.Id) == 0);
                foreach (var ellection in elections)
                {
                    AddBulletinRequest(ellection.Id, gm.UserId);
                }
            }
        }

        private static readonly ConcurrentQueue<KeyValuePair<Guid, Guid>> BulletinRequestQueue = new ConcurrentQueue<KeyValuePair<Guid, Guid>>();
        private delegate void PrintNewBulletinsCallback();
        private bool _isPrinting;

        private void PrintNewBulletins()
        {
            if (!_isPrinting)
            {
                _isPrinting = true;

                while (BulletinRequestQueue.Count != 0)
                {
                    KeyValuePair<Guid, Guid> request;
                    if (BulletinRequestQueue.TryDequeue(out request))
                    {
                        var votingId = request.Key;
                        var userId = request.Value;
                        var voting = DataService.PerThread.ContentSet.OfType<Voting>().SingleOrDefault(x => x.Id == votingId);

                        if (voting != null)
                        {
                            if (voting is Poll)
                            {
                                var poll = voting as Poll;
                                var bulletin = new PollBulletin();

                                if (poll.State == (byte)ContentState.Approved && !poll.IsFinished)
                                {
                                    var groupMember = GroupService.UserInGroup(userId, poll.Group);

                                    if (groupMember != null && (groupMember.State == (byte)GroupMemberState.Approved || groupMember.State == (byte)GroupMemberState.Moderator))
                                    {
                                        if (poll.Bulletins.Count(x => x.OwnerId == groupMember.Id) == 0)
                                        {
                                            bulletin.Owner = groupMember;
                                            bulletin.Weight = 1;
                                            poll.Bulletins.Add(bulletin);
                                            DataService.PerThread.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (voting is Election)
                            {
                                var election = voting as Election;
                                var bulletin = new ElectionBulletin();

                                if (election.State == (byte)ContentState.Approved && !election.IsFinished)
                                {
                                    var groupMember = GroupService.UserInGroup(userId, election.Group);

                                    if (groupMember != null && (groupMember.State == (byte)GroupMemberState.Approved || groupMember.State == (byte)GroupMemberState.Moderator))
                                    {
                                        if (election.ElectionBulletins.Count(x => x.OwnerId == groupMember.Id) == 0)
                                        {
                                            bulletin.Owner = groupMember;
                                            election.ElectionBulletins.Add(bulletin);
                                            DataService.PerThread.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Thread.Sleep(1);
                }

                _isPrinting = false;
            }
        }

        public void AnalizeGroupMemberBulletins(Guid groupMemberId)
        {
            AnalizeMissedBulletinsCallBack pollCallback = AnalizeMissedBulletins;
            pollCallback.BeginInvoke(groupMemberId, null, null);
        }

        public void AddBulletinRequest(Guid votingId, Guid userId)
        {
            BulletinRequestQueue.Enqueue(new KeyValuePair<Guid, Guid>(votingId, userId));
            PrintNewBulletinsCallback pollCallback = PrintNewBulletins;
            pollCallback.BeginInvoke(null, null);
        }

        #region Polls

        private delegate void PollCallback(Guid pollId);
        private readonly IDictionary<Guid, PollPublishDataContainer> _pollsInPublish = new Dictionary<Guid, PollPublishDataContainer>();

        public Poll CreatePoll(string groupUrl, Guid authorId, PollContainer pollData)
        {
            var group = GroupService.GetGroupByLabelOrId(groupUrl);
            var author = GroupService.UserInGroup(authorId, group.Id);

            if (group.State == (byte)GroupState.Blank)
                throw new BusinessLogicException("Нельзя создавать голосования в еще не оформленных группах");

            if (author == null)
                throw new BusinessLogicException("Вы не являетесь участником указанной группы");
            if (author.State == (byte)GroupMemberState.NotApproved)
                throw new BusinessLogicException("Вы еще не являетесь участником указанной группы");

            var poll = new Poll
            {
                GroupId = group.Id,
                Author = author.User,
                CreationDate = DateTime.Now,
                Title = pollData.Title,
                Text = pollData.Text,
                State = (byte)(pollData.IsDraft ? ContentState.Draft : ContentState.Premoderated),
                Duration = pollData.Duration,
                HasOpenProtocol = pollData.HasOpenProtocol
            };

            if (poll.State == (byte)ContentState.Premoderated)
                poll.PublishDate = DateTime.Now;

            foreach (var tag in pollData.Tags)
                if (group.Tags.Contains(tag))
                    poll.Tags.Add(tag);

            DataService.PerThread.SaveChanges();

            return poll;
        }

        public void UpdatePoll(Guid pollId, PollContainer pollData)
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == pollId);
            if (poll == null)
                throw new BusinessLogicException("Указанное голосование не найдено");

            if (poll.State == (byte)ContentState.Approved)
                throw new BusinessLogicException("Нельзя редактировать уже опубликованное голосование");

            poll.Title = pollData.Title;
            poll.Text = pollData.Text;
            poll.State = (byte)(pollData.IsDraft ? ContentState.Draft : ContentState.Premoderated);
            poll.Duration = pollData.Duration;

            if (poll.State == (byte) ContentState.Premoderated)
                poll.PublishDate = DateTime.Now;

            poll.Tags.Clear();
            foreach (var tag in pollData.Tags)
            {
                if (poll.Group.Tags.Contains(tag))
                    poll.Tags.Add(tag);
            }

            DataService.PerThread.SaveChanges();
        }

        public Poll StartPoll(Guid pollId, Guid userId)
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == pollId);
            if (poll == null)
                throw new BusinessLogicException("Указанное голосование не найдено. Убедитесь, что вы перешли по правильной ссылке.");

            var uig = GroupService.UserInGroup(userId, poll.Group.Id, true);
            if (uig.State != (byte)GroupMemberState.Moderator && uig.Group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration))
                throw new BusinessLogicException("Только модератор группы может запустить голосование");

            /*if ()
                throw new BusinessLogicException("Нельзя опубликовать уже опубликованное голосование");*/

            var publishData = GetPollPublishData(pollId);
            if (publishData != null && !publishData.IsFailed)
                throw new BusinessLogicException("Идет процесс печати бюллетеней, который начался " + publishData.BeganAt.ToString("dd.MM.yyyy HH:mm"));

            DataService.PerThread.SaveChanges();

            PollCallback pollCallback = CreateBulletins;
            pollCallback.BeginInvoke(poll.Id, null, null);

            return poll;
        }

        private void CreateBulletins(Guid pollId)
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == pollId);
            var data = new PollPublishDataContainer();

            if (poll == null)
            {
                data.Fail("Указанное голосование не найдено");
                return;
            }

            if (_pollsInPublish.ContainsKey(pollId))
                _pollsInPublish.Remove(pollId);

            _pollsInPublish.Add(pollId, data);

            // Начинаем печатать бюллетени
            try
            {
                var clearlist = DataService.PerThread.BulletinSet.OfType<PollBulletin>().Where(x => x.PollId == pollId).ToList();
                foreach (var bulletin in clearlist)
                    DataService.PerThread.BulletinSet.DeleteObject(bulletin);

                poll.Bulletins.Clear();

                DataService.PerThread.SaveChanges();

                var members = poll.Group.GroupMembers
                    .Where(x => !x.User.IsOutdated && (x.State == (byte) GroupMemberState.Approved || x.State == (byte) GroupMemberState.Moderator))
                    .ToList();

                // создаем пустые бюллетени
                foreach (var member in members)
                {
                    var bulletin = new PollBulletin
                    {
                        OwnerId = member.Id,
                        Weight = 1
                    };
                    poll.Bulletins.Add(bulletin);
                }

                // расставляем делегирование
                foreach (var member in members)
                {
                    var experts = member.ExpertVotes;
                    if (experts.Count > 0)
                    {
                        foreach (var expert in experts)
                        {
                            if (poll.Tags.Contains(expert.Tag))
                            {
                                var grantorBulletin = poll.Bulletins.Single(x => x.OwnerId == member.Id);
                                var expertBulletin = poll.Bulletins.Single(x => x.OwnerId == expert.Expert.GroupMember.Id);

                                if (!grantorBulletin.ExpertBulletins.Contains(expertBulletin))
                                    grantorBulletin.ExpertBulletins.Add(expertBulletin);
                            }
                        }
                    }
                }

                // распределяем вес
                foreach (var bulletin in poll.Bulletins)
                {
                    if (bulletin.GrantorBulletins.Count == 0)
                    {
                        if (bulletin.ExpertBulletins.Count == 1)
                        {
                            bulletin.ExpertBulletins.First().Weight += 1;
                            bulletin.Weight = 0;
                        }

                        // если несколько экспертов, то человек принимает решение сам, а решения экспертов носят рекомендательный характер
                        // но если при подсчете голосов эксперты солидарны и человек не голосовал сам, то считаем как считает большинство экспертов
                    }
                }

                poll.PublishDate = DateTime.Now;
                poll.State = (byte)ContentState.Approved;

                DataService.PerThread.SaveChanges();

                _pollsInPublish.Remove(poll.Id);

                if (poll.Duration.HasValue)
                    ScheduleService.AddJob(new FinishPollTask(poll.Id), poll.PublishDate.Value.AddDays(poll.Duration.Value), false, null, false);

                MessageService.SendToGroup(poll.Group, MessageComposer.ComposePollNotice(poll.Id), MessageType.PollNotice);
            }
            catch (Exception exp)
            {
                if (_pollsInPublish.ContainsKey(poll.Id))
                {
                    var info = _pollsInPublish[poll.Id];
                    info.Fail("Создание бюллетений прервано с ошибкой: " + exp.InnerException);
                }
            }
        }

        public PollPublishDataContainer GetPollPublishData(Guid pollId)
        {
            PollPublishDataContainer data = null;

            if (_pollsInPublish.ContainsKey(pollId))
                data = _pollsInPublish[pollId];

            return data;
        }

        public void VotePoll(Guid pollId, Guid userId, VoteOption voteOption, string voteComment)
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == pollId);
            if (poll == null)
                throw new BusinessLogicException("Указанное голосование не найдено");

            if (poll.State != (byte)ContentState.Approved)
                throw new BusinessLogicException("Голосование еще не запущено");
            if (poll.IsFinished)
                throw new BusinessLogicException("Голосование уже завершено");
            if (!poll.GroupId.HasValue)
                throw new BusinessLogicException("Голосование не привзяанно к группе");

            var member = GroupService.UserInGroup(userId, poll.GroupId.Value, true);

            var bulletin = DataService.PerThread.BulletinSet.OfType<PollBulletin>().SingleOrDefault(x => x.OwnerId == member.Id && x.PollId == poll.Id);
            if (bulletin == null)
            {
                AddBulletinRequest(pollId, userId);

                throw new BusinessLogicException("Система еще не напечатала на вас бюллетень. Попробуйте повторить через несколько минут.");
            }

            bulletin.Result = (byte)voteOption;
            bulletin.Comment = voteComment;

            DataService.PerThread.SaveChanges();
        }

        public void SummarizePoll(Guid pollId)
        {
            try
            {
                var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == pollId);
                if (poll == null)
                    throw new BusinessLogicException("Указанное голосование не найдено");

                if (poll.IsFinished)
                    throw new BusinessLogicException("Голосование уже завершено");
                if (poll.State != (byte)ContentState.Approved || !poll.PublishDate.HasValue)
                    throw new BusinessLogicException("Голосование еще не запущено");
                if (poll.Duration.HasValue && poll.PublishDate.Value.AddDays(poll.Duration.Value) > DateTime.Now)
                    throw new BusinessLogicException("Срок голосования еще не вышел");

                poll.IsFinished = true;

                var yes = 0;
                var no = 0;
                var refrained = 0;

                foreach (var bulletin in poll.Bulletins)
                {
                    if (bulletin.Result != (byte)VoteOption.NotVoted)
                    {
                        if (bulletin.Result == (byte)VoteOption.Yes)
                            yes += bulletin.Weight;
                        if (bulletin.Result == (byte)VoteOption.No)
                            no += bulletin.Weight;
                        if (bulletin.Result == (byte)VoteOption.Refrained)
                            refrained += bulletin.Weight;
                    }
                }

                string result;

                if (100 * (yes + no + refrained) / poll.Bulletins.Count < poll.Group.PollQuorum)
                {
                    poll.Result = (byte)VoteOption.NotVoted;
                    result = "Голосование не состоялось в связи с низкой явкой";
                }
                else
                {
                    if (yes == no)
                    {
                        poll.Result = (byte)VoteOption.Refrained;
                        result = "Решение не принято, т.к. голоса разделились поровну";
                    }
                    else if (yes > no)
                    {
                        poll.Result = (byte)VoteOption.Yes;
                        result = "Решение принято, большинство проголосовало За";
                    }
                    else
                    {
                        poll.Result = (byte)VoteOption.No;
                        result = "Решение принято, большинство проголосовало Против";
                    }
                }

                DataService.PerThread.SaveChanges();

                var reportsPath = ConstHelper.AppPath + @"MediaContent\Reports\";
                var reportName = pollId + ".csv";
                var path = Path.Combine(reportsPath, reportName);

                ReportExportService.ExportCSV(path, pollId);

                MessageService.SendToGroup(poll.Group, MessageComposer.ComposePollNotice(poll.Id, result), MessageType.PollNotice);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Elections

        private delegate void ElectionCallback(Guid electionId);
        private IList<Guid> _frozenElections = new List<Guid>();

        public Election CreateElection(string groupUrl, Guid? authorId)
        {
            var group = GroupService.GetGroupByLabelOrId(groupUrl);

            GroupService.CanElectionsBeStarted(group, true);

            if (authorId.HasValue)
            {
                var author = GroupService.UserInGroup(authorId.Value, group.Id);

                if (author == null || author.State == (byte)GroupMemberState.Banned || author.State == (byte)GroupMemberState.NotMember)
                    throw new BusinessLogicException("Вы не являетесь участником указанной группы");
                if (author.State == (byte)GroupMemberState.NotApproved)
                    throw new BusinessLogicException("Вы еще не являетесь участником указанной группы");
                if (author.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Вы не являетесь модератором указанной группы");
            }

            var electionNumber = group.Content.OfType<Election>().Count() + 1;

            var election = new Election
            {
                Group = group,
                AuthorId = authorId,
                CreationDate = DateTime.Now,
                PublishDate = DateTime.Now,
                Title = electionNumber + "-ые выборы модераторов",
                State = (byte)ContentState.Approved,
                Stage = (byte)ElectionStage.Agitation,
                AgitationDuration = ConstHelper.ElectionAgitationDurationDays,
                Duration = ConstHelper.ElectionDurationDays,
                Text = "Пришло время выбрать новых модераторов группы!",
                Quorum = (int)(group.GroupMembers.Count * ((float)group.ElectionQuorum / 100))
            };

            election.Tags.Add(TagService.GetTag("Очередные", group.Id, true)); // TODO: потом будут внеочередные и т.п.
            election.Tags.Add(TagService.GetTag("Выборы", group.Id, true));

            DataService.PerThread.SaveChanges();
            ContentService.Attach(election.Id, null, AttachDetachTarget.Group);


            MessageService.SendToGroup(group, new MessageStruct
                {
                    Date = DateTime.Now,
                    Text = string.Format("В группе <a href={0}>{1}</a> запущены <a href={2}>{3}</a>",
                        UrlHelper.GetUrl<Group>(group.Url), group.Name, UrlHelper.GetUrl<Voting>(election.Id), election.Title),
                    Type = (byte)MessageType.ElectionNotice
                }, GroupMessageRecipientType.Members | GroupMessageRecipientType.Moderators);

            return election;
        }

        public Candidate BecomeCandidate(Guid electionId, Guid userId)
        {
            Election election = DataService.PerThread.ContentSet.OfType<Election>().SingleOrDefault(x => x.Id == electionId);
            if (election == null)
                throw new BusinessLogicException("Не найдены выборы с указанным идентификатором");
            if (election.Stage != (byte)ElectionStage.Agitation)
                throw new BusinessLogicException("Период агитации уже закончился");

            GroupMember gm = GroupService.UserInGroup(userId, election.GroupId.Value);
            if (gm == null)
                throw new BusinessLogicException("Вы не состоите в данной группе");
            if (gm.State == (byte)GroupMemberState.NotApproved)
                throw new BusinessLogicException("Только подтвержденные члены группы могут выдвигаться в кандидаты");

            if (gm.Candidate != null && election.Candidates.Count(x => x.Id == gm.Candidate.Id) > 0)
            {
                throw new BusinessLogicException("Вы уже явлетесь кандидатом!");
            }

            Candidate candidate = gm.Candidate ?? new Candidate();

            candidate.ElectionId = electionId;
            candidate.GroupMember = gm;

            if (gm.State == (byte)GroupMemberState.Moderator)
            {
                candidate.Status = (byte)CandidateStatus.Confirmed;
            }
            else
            {
                candidate.Status = (byte)CandidateStatus.Declared;
                CreateCandidatePetition(candidate);
            }

            DataService.PerThread.SaveChanges();

            return candidate;
        }

        public Petition CreateCandidatePetition(Candidate candidate, string text = null)
        {
            var election = candidate.Election;
            var user = candidate.GroupMember.User;

            Petition petition = VotingService.CreatePetition(new PetitionContainer
            {
                GroupId = election.GroupId.Value,
                Title = "Выдвижение " + user.FullName + " в качестве кандидата на " + election.Title,
                Tags = "Выборы, Петиция кандидата",
                IsPrivate = true
            }, user.Id, false);

            if (string.IsNullOrEmpty(text))
                petition.Text = "Подпишите петицию, если вы хотите, чтобы я участвовал в выборах";
            else
                petition.Text = text;
            petition.State = (byte)ContentState.Approved;
            petition.PublishDate = DateTime.Now;
            petition.Candidate = candidate;

            DataService.PerThread.SaveChanges();

            return petition;
        }

        public Election StartElection(Guid electionId)
        {
            if (!_frozenElections.Contains(electionId))
            {
                _frozenElections.Add(electionId);

                Election election = DataService.PerThread.ContentSet.OfType<Election>().SingleOrDefault(x => x.Id == electionId);
                if (election == null)
                    throw new BusinessLogicException("Не найдены выборы с указанным идентификатором");
                if (election.Stage == (byte)ElectionStage.Voting)
                    throw new BusinessLogicException("Уже идет голосование");
                if (election.IsFinished)
                    throw new BusinessLogicException("Голосование уже закончилось");

                ElectionCallback electionCallback = _StartElection;
                electionCallback.BeginInvoke(election.Id, null, null);

                MessageService.SendToGroup(election.Group, new MessageStruct
                {
                    Date = DateTime.Now,
                    Text = string.Format("В группе <a href={0}>{1}</a> началось голосование в <a href={2}>{3}</a>",
                        UrlHelper.GetUrl<Group>(election.Group.Url), election.Group.Name, UrlHelper.GetUrl<Voting>(election.Id), election.Title),
                    Type = (byte)MessageType.ElectionNotice
                }, GroupMessageRecipientType.Members | GroupMessageRecipientType.Moderators);

                _frozenElections.Remove(electionId);

                return election;
            }

            return null;
        }

        private void _StartElection(Guid electionId)
        {
            var election = DataService.PerThread.ContentSet.OfType<Election>().Single(x => x.Id == electionId);

            try
            {
                election.ElectionBulletins.Clear();
                foreach (var member in election.Group.GroupMembers.Where(x => (x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator) && !x.User.IsOutdated))
                {
                    var bulletin = new ElectionBulletin
                    {
                        OwnerId = member.Id,
                        ElectionId = electionId
                    };
                    election.ElectionBulletins.Add(bulletin);
                }

                foreach (var candidate in election.Candidates.Where(x => x.Status == (byte)CandidateStatus.Declared))
                {
                    if (candidate.Petition.Signers.Count >= ConstHelper.CandidatePetitionNecessarySigners)
                        candidate.Status = (byte)CandidateStatus.Confirmed;
                }

                if (election.Candidates.Count(x => x.Status == (byte)CandidateStatus.Confirmed) < election.Group.ModeratorsCount)
                {
                    _frozenElections.Remove(electionId);
                    FinishElection(election.Id);
                }
                else
                    election.Stage = (byte)ElectionStage.Voting;

                DataService.PerThread.SaveChanges();
                ScheduleService.AddJob(new FinishElectionTask(election.Id), DateTime.Now.AddDays(election.Duration.Value), false, null, true);
            }
            catch (Exception exp)
            {
                _frozenElections.Remove(electionId);

                throw new BusinessLogicException("Создание бюллетеней прервано с ошибкой: " + exp.InnerException);
            }
        }

        public Election ElectionVote(Guid electionId, Guid userId, IList<Guid> candidates)
        {
            Election election = DataService.PerThread.ContentSet.OfType<Election>().SingleOrDefault(x => x.Id == electionId);
            if (election == null)
                throw new BusinessLogicException("Не найдены выборы с указанным идентификатором");
            if (election.Stage == (byte)ElectionStage.Agitation)
                throw new BusinessLogicException("Голосование еще не началось");
            if (election.IsFinished)
                throw new BusinessLogicException("Голосование уже закончилось");

            GroupMember gm = GroupService.UserInGroup(userId, election.GroupId.Value);
            if (gm == null)
                throw new BusinessLogicException("Вы не можете голосовать, т.к. не состоите в группе, в которой проходят данные выборы");
            if (gm.State == (byte)GroupMemberState.NotApproved)
                throw new BusinessLogicException("Вы не можете голосовать, т.к. еще не состоите в группе, в которой проходят данные выборы");
            if (election.ElectionBulletins.Count(x => x.OwnerId == gm.Id) == 0)
            {
                AddBulletinRequest(electionId, userId);
                throw new BusinessLogicException("Система еще не напечатала на вас бюллетень. Попробуйте повторить через несколько минут.");
            }

            if (candidates.Count > election.Group.ModeratorsCount)
                throw new BusinessLogicException("Вы выбрали слишком много кандидатов. Можно голосовать не более чем за " +
                    DeclinationService.OfNumber(election.Group.ModeratorsCount, "человека", "человек", "человек"));

            ElectionBulletin bulletin = gm.Bulletins.OfType<ElectionBulletin>().SingleOrDefault(x => x.ElectionId == electionId);
            if (bulletin.Result.Count > 0)
                throw new BusinessLogicException("Вы уже голосовали на данных выборах");

            foreach (var candidateId in candidates)
            {
                Candidate candidate = DataService.PerThread.CandidateSet.SingleOrDefault(x => x.Id == candidateId);
                if (candidate == null)
                    throw new BusinessLogicException("Неверно указан идентификатор одного из кандидатов");

                if (candidate.Status == (byte)CandidateStatus.Confirmed)
                    bulletin.Result.Add(candidate);
            }

            DataService.PerThread.SaveChanges();

            return election;
        }

        public void FinishElection(Guid electionId)
        {
            var election = DataService.PerThread.ContentSet.OfType<Election>().SingleOrDefault(x => x.Id == electionId);
            if (election == null)
                throw new BusinessLogicException("Не найдены выборы с указанным идентификатором");
            if (election.IsFinished)
                throw new BusinessLogicException("Выборы уже завершены");

            try
            {
                if (election.Candidates.Count(x => x.Status == (byte)CandidateStatus.Confirmed) < election.Group.ModeratorsCount)
                    election.Stage = (byte)ElectionStage.Failed;
                else
                    if (election.Turnout >= election.Quorum)
                    {
                        var winners = election.Candidates
                            .Where(x => x.Status == (byte)CandidateStatus.Confirmed)
                            .OrderByDescending(x => x.Electorate.Count)
                            .Take(election.Group.ModeratorsCount);

                        var oldModers = election.Group.GroupMembers.Where(x => x.State == (byte)GroupMemberState.Moderator);
                        foreach (var oldModer in oldModers)
                            oldModer.State = (byte)GroupMemberState.Approved;

                        foreach (var winner in winners)
                        {
                            winner.Status = (byte)CandidateStatus.Winner;
                            winner.GroupMember.State = (byte)GroupMemberState.Moderator;
                        }

                        election.Stage = (byte)ElectionStage.Completed;
                    }
                    else
                        election.Stage = (byte)ElectionStage.Failed;

                election.IsFinished = true;
                if (election.GroupId.HasValue)
                    CachService.DropViewModelByModel(election.GroupId.Value);

                DataService.PerThread.SaveChanges();

                ScheduleService.AddJob(new UnbindElectionTask(election.Id), DateTime.Now.AddDays(1), false, null, true);
            }
            catch (Exception exp)
            {
                throw new BusinessLogicException("Завершение выборов прервано с ошибкой: " + exp.InnerException);
            }
        }

        #endregion

        #region Surveys

        public Survey CreateSurvey(SurveyData data, Guid? userId)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == data.GroupId);
            if (group == null)
                throw new BusinessLogicException("Указан неверный идентификатор группы");

            if (userId.HasValue)
                GroupService.UserInGroup(userId.Value, group, true);

            if (data.VariantsCount >= data.Options.Count)
                throw new BusinessLogicException("Разрешено выбирать слишком много вариантов");

            var survey = new Survey
            {
                AuthorId = userId,
                Title = data.Title,
                Text = data.Text,
                IsPrivate = data.IsPrivate,
                Tags = TagsHelper.ConvertStringToTagList(data.Tags, data.GroupId),
                GroupId = data.GroupId,
                Duration = data.Duration,
                VariantsCount = data.VariantsCount,
                CreationDate = DateTime.Now,
                State = data.IsDraft ? (byte)ContentState.Draft : (byte)ContentState.Premoderated,
                HasOpenProtocol = data.HasOpenProtocol
            };

            var options = new List<Option>();
            byte optPos = 0;
            foreach (var o in data.Options)
            {
                var option = new Option
                {
                    Position = optPos,
                    Title = o.Title,
                    Description = o.Description,
                    SurveyId = survey.Id
                };
                options.Add(option);
                optPos++;
            }
            survey.Options = options;

            DataService.PerThread.ContentSet.AddObject(survey);
            DataService.PerThread.SaveChanges();

            return survey;
        }

        public Survey UpdateSurvey(SurveyData data, Guid? userId)
        {
            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == data.Id);
            if (survey == null)
                throw new BusinessLogicException("Указан неверный идентификатор опроса");

            return UpdateSurvey(survey, data, userId);
        }

        public Survey UpdateSurvey(Survey survey, SurveyData data, Guid? userId)
        {
            if (survey.State != (byte)ContentState.Draft && survey.State != (byte)ContentState.Premoderated)
                throw new BusinessLogicException("Данный опрос нельзя изменять");

            if (userId.HasValue)
            {
                if (survey.AuthorId != userId)
                    throw new BusinessLogicException("Вы не являетесь автором опроса");

                GroupService.UserInGroup(userId.Value, survey.Group, true);
            }

            if (data.VariantsCount >= data.Options.Count)
                throw new BusinessLogicException("Разрешено выбирать слишком много вариантов");

            survey.Title = data.Title;
            survey.Text = data.Text;
            survey.IsPrivate = data.IsPrivate;
            survey.Duration = data.Duration;
            survey.VariantsCount = data.VariantsCount;
            survey.Tags.Clear();

            foreach (var tag in TagsHelper.ConvertStringToTagList(data.Tags, survey.GroupId))
                survey.Tags.Add(tag);

            var optionsForDelete = survey.Options.ToList();
            foreach (var option in optionsForDelete)
                DataService.PerThread.OptionSet.DeleteObject(option);

            var options = new List<Option>();
            byte optPos = 0;
            foreach (var o in data.Options)
            {
                var option = new Option
                {
                    Position = optPos,
                    Title = o.Title,
                    Description = o.Description,
                    SurveyId = survey.Id
                };
                DataService.PerThread.OptionSet.AddObject(option);
                options.Add(option);
                optPos++;
            }

            survey.Options.Clear();
            foreach (var option in options)
                survey.Options.Add(option);

            DataService.PerThread.SaveChanges();

            return survey;
        }

        public Survey StartSurvey(Guid surveyId)
        {
            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == surveyId);
            if (survey == null)
                throw new BusinessLogicException("Указан неверный идентификатор опроса");

            if (!survey.PublishDate.HasValue)
                throw new BusinessLogicException("Опрос еще не опубликован");

            if (survey.Duration.HasValue)
                ScheduleService.AddJob(new FinishSurveyTask(survey.Id), survey.PublishDate.Value.AddDays(survey.Duration.Value), false, null, false);

            DataService.PerThread.SaveChanges();

            MessageService.SendToGroup(survey.Group, new MessageStruct
            {
                Date = DateTime.Now,
                Text = string.Format("В группе <a href={0}>{1}</a> начался опрос в <a href={2}>{3}</a>",
                    UrlHelper.GetUrl<Group>(survey.Group.Url), survey.Group.Name, UrlHelper.GetUrl<Voting>(survey.Id), survey.Title),
                Type = (byte)MessageType.ElectionNotice
            }, GroupMessageRecipientType.Members | GroupMessageRecipientType.Moderators);

            return survey;
        }

        private static Survey SurveyVote(Survey survey, ICollection<Option> options, Guid userId)
        {
            if (survey.IsFinished)
                throw new BusinessLogicException("Опрос уже завершен");
            if (survey.IsPrivate)
                GroupService.UserInGroup(userId, survey.Group, true);

            var bulletin = survey.Bulletins.SingleOrDefault(x => x.UserId == userId);
            if (bulletin != null)
                throw new BusinessLogicException("Вы уже голосовали");

            bulletin = new SurveyBulletin
            {
                UserId = userId,
                SurveyId = survey.Id,
                Result = options
            };
            DataService.PerThread.SurveyBulletinSet.AddObject(bulletin);
            DataService.PerThread.SaveChanges();

            return survey;
        }

        public Survey SurveyVote(Guid optionId, Guid userId)
        {
            var option = DataService.PerThread.OptionSet.SingleOrDefault(x => x.Id == optionId);
            if (option == null)
                throw new BusinessLogicException("Указан неверный идентификатор варианта");

            return SurveyVote(option.Survey, new[] { option }, userId);
        }

        public Survey SurveyVote(Guid surveyId, ICollection<Guid> optionIds, Guid userId)
        {
            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == surveyId);
            if (survey == null)
                throw new BusinessLogicException("Указан неверный идентификатор опроса");

            if (optionIds.Count() > survey.VariantsCount)
                throw new BusinessLogicException("Выбрано слишком много вариантов");

            var options = new List<Option>();
            if (optionIds != null)
                foreach (var oId in optionIds)
                {
                    var option = DataService.PerThread.OptionSet.SingleOrDefault(x => x.Id == oId);
                    if (option == null)
                        throw new BusinessLogicException("Указан неверный идентификатор варианта");

                    if (survey != option.Survey)
                        throw new BusinessLogicException("Выбранные варианты должны относиться к одному опросу");

                    options.Add(option);
                }

            return SurveyVote(survey, options, userId);
        }

        public Survey SurveyNotVote(Guid surveyId, Guid userId)
        {
            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == surveyId);
            if (survey == null)
                throw new BusinessLogicException("Указан неверный идентификатор опроса");

            return SurveyVote(survey, null, userId);
        }

        public Survey FinishSurvey(Guid surveyId)
        {
            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == surveyId);
            if (survey == null)
                throw new BusinessLogicException("Указан неверный идентификатор опроса");

            if (!survey.PublishDate.HasValue || survey.State != (byte)ContentState.Approved)
                throw new BusinessLogicException("Данный опрос еще даже не опубликован");
            if (survey.Duration.HasValue && survey.PublishDate.Value.AddDays(survey.Duration.Value) > DateTime.Now)
                throw new BusinessLogicException("Нельзя завершать опрос раньше времени");

            survey.IsFinished = true;
            DataService.PerThread.SaveChanges();

            return survey;
        }

        #endregion
    }
}
