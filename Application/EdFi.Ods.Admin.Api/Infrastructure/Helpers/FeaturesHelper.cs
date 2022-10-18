using EdFi.Ods.Admin.Api.Features;
using System.Reflection;

namespace EdFi.Ods.Admin.Api.Infrastructure.Helpers
{
    public class FeaturesHelper
    {
        public static List<IFeature> GetFeatures()
        {
            var featureInterface = typeof(IFeature);
            var featureImpls = Assembly.GetExecutingAssembly().GetTypes()
                .Where(p => featureInterface.IsAssignableFrom(p) && p.IsClass);

            var features = new List<IFeature>();

            foreach (var featureImpl in featureImpls)
            {
                if (Activator.CreateInstance(featureImpl) is IFeature feature)
                    features.Add(feature);
            }
            return features;
        }
    }
}
