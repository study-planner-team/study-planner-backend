using StudyPlannerAPI.Models.StudyMaterials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.Plans
{
    public static class StudyMaterialFactory
    {
        public static StudyMaterial CreateStudyMaterial(int materialId, int topicId, string title, string link)
        {
            return new StudyMaterial
            {
                StudyMaterialId = materialId,
                Title = title,
                Link = link,
                StudyTopicId = topicId
            };
        }
    }
}
