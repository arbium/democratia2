using System;
using System.Collections.Generic;

namespace Federation.Core
{
    public static class VotingService
    {
        private static readonly IVotingService _votingService = new VotingServiceImpl();

        public static Petition CreatePetition(PetitionContainer data, Guid userId, bool saveChanges = true)
        {
            return _votingService.CreatePetition(data, userId, saveChanges);
        }

        public static Petition EditPetition(PetitionContainer data, Guid userId, bool saveChanges = true)
        {
            return _votingService.EditPetition(data, userId, saveChanges);
        }

        public static Petition SignPetition(Guid id, Guid userId, bool saveChanges = true)
        {
            return _votingService.SignPetition(id, userId, saveChanges);
        }

        public static Petition PublishPetition(Guid id, Guid userId, bool saveChanges = true)
        {
            return _votingService.PublishPetition(id, userId, saveChanges);
        }

        public static Coauthor InvitePetitionCoauthor(PetitionCoauthorContainer data, Guid userId, bool saveChanges = true)
        {
            return _votingService.InvitePetitionCoauthor(data, userId, saveChanges);
        }

        public static Petition DeletePetitionCoauthor(Guid id, Guid userId, bool saveChanges = true)
        {
            return _votingService.DeletePetitionCoauthor(id, userId, saveChanges);
        }

        public static Petition RespondToPetitionInvite(Guid id, Guid userId, bool accept, bool saveChanges = true)
        {
            return _votingService.RespondToPetitionInvite(id, userId, accept, saveChanges);
        }

        public static void FinishPetition(Guid id)
        {
            _votingService.FinishPetition(id);
        }



        public static void AddBulletinRequest(Guid votingId, Guid userId)
        {
            _votingService.AddBulletinRequest(votingId, userId);
        }

        public static void AnalizeGroupMemberBulletins(Guid groupMemberId)
        {
            _votingService.AnalizeGroupMemberBulletins(groupMemberId);
        }

        public static Poll CreatePoll(string groupUrl, Guid authorId, PollContainer pollData)
        {
            return _votingService.CreatePoll(groupUrl, authorId, pollData);
        }

        public static void UpdatePoll(Guid pollId, PollContainer pollData)
        {
            _votingService.UpdatePoll(pollId, pollData);
        }

        public static Poll StartPoll(Guid pollId, Guid userId)
        {
            return _votingService.StartPoll(pollId, userId);
        }

        public static PollPublishDataContainer GetPollPublishData(Guid pollId)
        {
            return _votingService.GetPollPublishData(pollId);
        }

        public static void VotePoll(Guid pollId, Guid userId, VoteOption voteOption, string voteComment)
        {
            _votingService.VotePoll(pollId, userId, voteOption, voteComment);
        }

        public static void SummarizePoll(Guid pollId)
        {
            _votingService.SummarizePoll(pollId);
        }



        public static Election CreateElection(string groupUrl, Guid? authorId)
        {
            return _votingService.CreateElection(groupUrl, authorId);
        }

        public static Candidate BecomeCandidate(Guid electionId, Guid userId)
        {
            return _votingService.BecomeCandidate(electionId, userId);
        }

        public static Petition CreateCandidatePetition(Candidate candidate, string text = null)
        {
            return _votingService.CreateCandidatePetition(candidate, text);
        }

        public static Election StartElection(Guid electionId)
        {
            return _votingService.StartElection(electionId);
        }

        public static Election ElectionVote(Guid electionId, Guid userId, IList<Guid> candidates)
        {
            return _votingService.ElectionVote(electionId, userId, candidates);
        }

        public static void FinishElection(Guid electionId)
        {
            _votingService.FinishElection(electionId);
        }



        public static Survey CreateSurvey(SurveyData surveyData, Guid userId)
        {
            return _votingService.CreateSurvey(surveyData, userId);
        }

        public static Survey UpdateSurvey(SurveyData data, Guid? userId)
        {
            return _votingService.UpdateSurvey(data, userId);
        }

        public static Survey UpdateSurvey(Survey survey, SurveyData data, Guid? userId)
        {
            return _votingService.UpdateSurvey(survey, data, userId);
        }

        public static Survey StartSurvey(Guid surveyId)
        {
            return _votingService.StartSurvey(surveyId);
        }

        public static Survey FinishSurvey(Guid surveyId)
        {
            return _votingService.FinishSurvey(surveyId);
        }

        public static Survey SurveyVote(Guid surveyId, ICollection<Guid> optionIds, Guid userId)
        {
            return _votingService.SurveyVote(surveyId, optionIds, userId);
        }

        public static Survey SurveyVote(Guid optionId, Guid userId)
        {
            return _votingService.SurveyVote(optionId, userId);
        }

        public static Survey SurveyNotVote(Guid surveyId, Guid userId)
        {
            return _votingService.SurveyNotVote(surveyId, userId);
        }
    }
}