using AutoMapper;
using StudyPlannerAPI.Mapper;

namespace StudyPlannerTests.Common
{
    public static class MapperFactory
    {
        public static IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(config =>
            {
                config.AddProfile<AutoMapperProfile>(); 
            });

            return configuration.CreateMapper();
        }
    }
}
