using System;
using System.Drawing;
using System.Linq;
using AxonInformer;

namespace Federation.Core
{
    [Serializable]
    public class BannerRegenerateTask : ScheduleTask
    {
        public override void Execute()
        {
            const string groupName = "party";

            var sourcePath = ConstHelper.AppPath + "MediaContent\\GroupBanners\\" + groupName + "\\src\\";
            var outputPath = ConstHelper.AppPath + "MediaContent\\GroupBanners\\" + groupName+"\\";

            var group = GroupService.GetGroupByLabelOrId(groupName);
            var groupCount = DataService.PerThread.GroupMemberSet.Count(x => x.GroupId == group.Id);

            Draw1(sourcePath, outputPath, "нас уже " + groupCount, new Size(200, 300), 13, 147);
            Draw2(sourcePath, outputPath, "нас уже " + groupCount, new Size(234, 60), 48, 30);
            Draw3(sourcePath, outputPath, "нас уже " + groupCount, new Size(240, 100), 55, 28);
            Draw4(sourcePath, outputPath, "Нас уже " + groupCount, new Size(468, 60), 55, 28);
        }

        private void Draw1(string sourcePath, string outputPath, string imageText, Size size, float x1, float y1)
        {
            var fileName = string.Format("{0}x{1}.png", size.Width, size.Height);
            var informer = new Informer(sourcePath + fileName);

            informer.DrawText("Партийный проект", Color.White, x1, y1 - 22);
            informer.DrawText(imageText, Color.White, x1, y1);
            informer.SaveToPng(outputPath + fileName);
        }

        private void Draw2(string sourcePath, string outputPath, string imageText, Size size, float x1, float y1)
        {
            var fileName = string.Format("{0}x{1}.png", size.Width, size.Height);
            var informer = new Informer(sourcePath + fileName);

            informer.DrawText("Партийный проект", Color.White, x1, y1 - 22);
            informer.DrawText(imageText, Color.White, x1, y1);
            informer.SaveToPng(outputPath + fileName);
        }

        private void Draw3(string sourcePath, string outputPath, string imageText, Size size, float x1, float y1)
        {
            var fileName = string.Format("{0}x{1}.png", size.Width, size.Height);
            var informer = new Informer(sourcePath + fileName);

            informer.DrawText("Партийный проект", Color.White, x1, y1 - 22);
            informer.DrawText(imageText, Color.White, x1, y1);
            informer.SaveToPng(outputPath + fileName);
        }

        private void Draw4(string sourcePath, string outputPath, string imageText, Size size, float x1, float y1)
        {
            var fileName = string.Format("{0}x{1}.png", size.Width, size.Height);
            var informer = new Informer(sourcePath + fileName);

            informer.DrawText("Партийный проект. " + imageText, Color.White, x1, y1 - 22);
            informer.SaveToPng(outputPath + fileName);
        }
    }
}