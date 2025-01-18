using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.StudyTopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.Plans
{
    public static class StudyTopicFactory
    {
        public static StudyTopic CreateStudyTopic(int topicId, int studyPlanId, string title, double hours, bool includeMaterials = true)
        {
            var studyTopic = new StudyTopic
            {
                TopicId = topicId,
                StudyPlanId = studyPlanId,
                Title = title,
                Hours = hours,
                StudyMaterials = includeMaterials
                    ? new List<StudyMaterial>
                    {
                        new StudyMaterial { StudyMaterialId = topicId * 10 + 1, Title = "Material 1", Link = "http://example.com/material1", StudyTopicId = topicId },
                        new StudyMaterial { StudyMaterialId = topicId * 10 + 2, Title = "Material 2", Link = "http://example.com/material2", StudyTopicId = topicId }
                    }
                    : new List<StudyMaterial>()
            };

            return studyTopic;
        }

    }
}
