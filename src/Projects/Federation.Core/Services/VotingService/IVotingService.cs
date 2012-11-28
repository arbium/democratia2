using System;
using System.Collections.Generic;

namespace Federation.Core
{
    public interface IVotingService
    {
        Petition CreatePetition(PetitionContainer data, Guid userId, bool saveChanges);
        Petition EditPetition(PetitionContainer data, Guid userId, bool saveChanges);
        Petition SignPetition(Guid id, Guid userId, bool saveChanges);
        Petition PublishPetition(Guid id, Guid userId, bool saveChanges);
        Coauthor InvitePetitionCoauthor(PetitionCoauthorContainer data, Guid userId, bool saveChanges);
        Petition DeletePetitionCoauthor(Guid id, Guid userId, bool saveChanges);
        Petition RespondToPetitionInvite(Guid id, Guid userId, bool accept, bool saveChanges);
        void FinishPetition(Guid id);

        void AnalizeGroupMemberBulletins(Guid groupMemberId);
        void AddBulletinRequest(Guid votingId, Guid userId);

        Poll CreatePoll(string groupUrl, Guid AuthorId, PollContainer pollData);
        void UpdatePoll(Guid pollId, PollContainer pollData);
        Poll StartPoll(Guid pollId, Guid userId);
        PollPublishDataContainer GetPollPublishData(Guid pollId);
        void VotePoll(Guid pollId, Guid userId, VoteOption voteOption, string voteComment);
        void SummarizePoll(Guid pollId);

        Election CreateElection(string groupUrl, Guid? authorId);
        Candidate BecomeCandidate(Guid electionId, Guid userId);
        Petition CreateCandidatePetition(Candidate candidate, string text = null);
        Election StartElection(Guid electionId);
        Election ElectionVote(Guid electionId, Guid userId, IList<Guid> candidates);
        void FinishElection(Guid electionId);

        Survey CreateSurvey(SurveyData data, Guid? userId);
        Survey UpdateSurvey(SurveyData data, Guid? userId);
        Survey UpdateSurvey(Survey survey, SurveyData data, Guid? userId);
        Survey StartSurvey(Guid surveyId);
        Survey FinishSurvey(Guid surveyId);
        Survey SurveyVote(Guid surveyId, ICollection<Guid> optionIds, Guid userId);
        Survey SurveyVote(Guid optionId, Guid userId);
        Survey SurveyNotVote(Guid surveyId, Guid userId);
    }
}
