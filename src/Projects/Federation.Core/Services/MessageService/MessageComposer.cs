using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Federation.Core
{
    public static class MessageComposer
    {
        public static string ComposeCommentNotice(Guid commentId, string text)
        {
            var comment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == commentId);
            if (comment == null)
                throw new BusinessLogicException("Комментарий не найден!");

            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\CommentNotice.html", Encoding.UTF8);

            var commentAuthorAvatar = ImageService.GetImageUrl<User>(comment.User.Avatar, false);
            var commentAuthorLink = UrlHelper.GetUrl<User>(comment.UserId, false);
            var commentAuthorName = comment.User.SurName + " " + comment.User.FirstName;
            var contentLink = comment.Content.GetUrl(comment, false);
            var contentName = comment.Content.Title;
            var answerLink = comment.GetUrl(false);

            return String.Format(template, commentAuthorAvatar, 
                commentAuthorLink, commentAuthorName, contentLink, contentName, TextHelper.CleanTags(text), answerLink);
        }

        public static string ComposeGroupModeratorNotice(Guid groupId, string text)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == groupId);
            if (group == null)
                throw new BusinessLogicException("Группа не найдена!");

            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\GroupModeratorNotice.html", Encoding.UTF8);

            var groupLogo = ImageService.GetImageUrl<Group>(group.Logo, false);
            var groupUrl = UrlHelper.GetUrl<Group>(group.Url, false);
            var groupName = group.Name;

            return String.Format(template, groupLogo, groupUrl, groupName, text);
        }

        public static string ComposePetitionNotice(Guid petitionId, string text)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == petitionId);
            if (petition == null)
                throw new BusinessLogicException("Петиция не найдена!");

            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\PetitionNotice.html", Encoding.UTF8);

            var authorAvatar = ImageService.GetImageUrl<User>(petition.Author.Avatar, false);
            var petitionUrl = petition.GetUrl(false);
            var petitionName = petition.Title;

            return String.Format(template, authorAvatar, petitionUrl, petitionName, text);
        }

        public static string ComposePollNotice(Guid pollId, string result = "")
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(p => p.Id == pollId);
            if (poll == null)
                throw new BusinessLogicException("Голосование не найдено!");

            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\PollNotice.html", Encoding.UTF8);

            var newPoll = "";
            var summary = "";
            string linkText;

            if (!poll.IsFinished)
            {
                newPoll = " - новое голосование";
                linkText = "Принять участие в голосовании";
            }
            else
            {
                var yes = poll.Bulletins.Count(x => x.Result == (byte)VoteOption.Yes);
                var no = poll.Bulletins.Count(x => x.Result == (byte)VoteOption.No);
                var not = poll.Bulletins.Count(x => x.Result == (byte)VoteOption.NotVoted);
                var forefit = poll.Bulletins.Count(x => x.Result == (byte)VoteOption.Refrained);

                summary = string.Format("Проголосовало {0} из {1} пользователей. Из них {2} За, {3} Против, {4} Воздержалось", poll.Bulletins.Count - not, poll.Bulletins.Count, yes, no, forefit);
                linkText = "Перейти к результатам";
            }

            var group = poll.Group;
            var groupLogo = ImageService.GetImageUrl<Group>(group.Logo, false);
            var groupUrl = UrlHelper.GetUrl<Group>(group.Url, false);
            var groupName = group.Name;
            var pollUrl = poll.GetUrl(false);
            var pollName = poll.Title;

            return String.Format(template, groupLogo, groupUrl, groupName, pollUrl, pollName, summary, result, newPoll, linkText);
        }

        public static string ComposeElectionNotice(Guid electionId, string result = "")
        {
            var election = DataService.PerThread.ContentSet.OfType<Election>().SingleOrDefault(p => p.Id == electionId);
            if (election == null)
                throw new BusinessLogicException("Выборы не найдены!");

            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\ElectionNotice.html", Encoding.UTF8);

            var newElection = "";
            var summary = "";
            string linkText;

            if (!election.IsFinished)
            {
                newElection = " - новые выборы";
                linkText = "Принять участие в выборах";
            }
            else
            {
                // Нужно сформулировать результат опроса, я ХЗ что тут писать
                summary = "";
                //var yes = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.Yes);
                //var no = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.No);
                //var not = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.NotVoted);
                //var forefit = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.Refrained);

                //summary = string.Format("Проголосовало {0} из {1} пользователей. Из них {2} За, {3} Против, {4} Воздержалось", poll.Bulletins.Count - not, poll.Bulletins.Count, yes, no, forefit);
                linkText = "Перейти к результатам";
            }

            var group = election.Group;
            var groupLogo = ImageService.GetImageUrl<Group>(group.Logo, false);
            var groupUrl = UrlHelper.GetUrl<Group>(group.Url, false);
            var groupName = group.Name;
            var pollUrl = election.GetUrl(false);
            var pollName = election.Title;

            return String.Format(template, groupLogo, groupUrl, groupName, pollUrl, pollName, summary, result, newElection, linkText);
        }


        public static string ComposeSurveyNotice(Guid surveyId, string result = "")
        {
            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(p => p.Id == surveyId);
            if (survey == null)
                throw new BusinessLogicException("Опрос не найден!");

            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\SurveyNotice.html", Encoding.UTF8);

            var newSurvey = "";
            var summary = "";
            string linkText;

            if (!survey.IsFinished)
            {
                newSurvey = " - новый опрос";
                linkText = "Принять участие в опросе";
            }
            else
            {
                // Нужно сформулировать результат опроса, я ХЗ что тут писать
                summary = "";
                //var yes = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.Yes);
                //var no = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.No);
                //var not = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.NotVoted);
                //var forefit = survey.Bulletins.Count(x => x.Result == (byte)VoteOption.Refrained);

                //summary = string.Format("Проголосовало {0} из {1} пользователей. Из них {2} За, {3} Против, {4} Воздержалось", poll.Bulletins.Count - not, poll.Bulletins.Count, yes, no, forefit);
                linkText = "Перейти к результатам";
            }

            var group = survey.Group;
            var groupLogo = ImageService.GetImageUrl<Group>(group.Logo, false);
            var groupUrl = UrlHelper.GetUrl<Group>(group.Url, false);
            var groupName = group.Name;
            var pollUrl = survey.GetUrl(false);
            var pollName = survey.Title;

            return String.Format(template, groupLogo, groupUrl, groupName, pollUrl, pollName, summary, result, newSurvey, linkText);
        }

        public static string ComposeSystemMessage(string text)
        {
            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\SystemMessage.html", Encoding.UTF8);

            return String.Format(template, text);
        }

        public static string ComposeInvitationMessage(Guid inviteGiverId, string invitedUserName, string inviteKey)
        {
            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\InvitationMessage.html", Encoding.UTF8);

            var inviteGiver = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == inviteGiverId);
            if (inviteGiver == null)
                throw new BusinessLogicException("Пользователь не найден!");

            var inviteGiverName = inviteGiver.FullName;
            var inviteGiverLink = UrlHelper.GetUrl<User>(inviteGiverId.ToString(), false);

            DataService.PerThread.EndWork();

            return String.Format(template, invitedUserName, inviteGiverLink, inviteGiverName, inviteKey);
        }

        public static string ComposeInvitationMessage(string invitedUserName, string inviteKey)
        {
            var template = File.ReadAllText(ConstHelper.AppPath + "\\MessageTemplates\\AdminInvitationMessage.html", Encoding.UTF8);

            return String.Format(template, invitedUserName, inviteKey);
        }
    }
}
