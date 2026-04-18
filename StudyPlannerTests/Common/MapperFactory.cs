using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
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
            }, NullLoggerFactory.Instance);

            return configuration.CreateMapper();
        }
    }
}