using System;
using System.Linq;
using Kent.Boogaart.KBCsv;


namespace Federation.Core
{
    public static class ReportExportService
    {
        public static void ExportCSV(string path, Guid pollId)
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == pollId);
            if (poll == null)
            {
                throw new BusinessLogicException("Указан неверный идентификатор голосования");
            }
            else
            {
                var bulletins = poll.Bulletins.OrderByDescending(x => x.Weight).ThenByDescending(x => x.Result).ThenBy(x => x.Id).ToList();
                using (var writer = new CsvWriter(path))
                {
                    writer.ValueSeparator = ';';
                    writer.WriteHeaderRecord("Номер бюллетеня", "Голос", "Вес");

                    foreach (var pollBulletin in bulletins)
                    {
                        if (pollBulletin.Weight.Equals(0))
                        {
                            writer.WriteDataRecord(pollBulletin.Id, "Голос делегирован", "-");
                        }
                        else
                        {

                            switch ((VoteOption)pollBulletin.Result)
                            {
                                case VoteOption.Yes:
                                    writer.WriteDataRecord(pollBulletin.Id, "За", pollBulletin.Weight);
                                    break;
                                case VoteOption.No:
                                    writer.WriteDataRecord(pollBulletin.Id, "Против", pollBulletin.Weight);
                                    break;
                                case VoteOption.NotVoted:
                                    writer.WriteDataRecord(pollBulletin.Id, "Не голосовал", pollBulletin.Weight);
                                    break;
                                case VoteOption.Refrained:
                                    writer.WriteDataRecord(pollBulletin.Id, "Воздержался", pollBulletin.Weight);
                                    break;
                                default:
                                    writer.WriteDataRecord(pollBulletin.Id, "Неизвестно", pollBulletin.Weight);
                                    break;
                            }
                        }
                    }
                    writer.Close();
                }
            }
        }
    }
}
