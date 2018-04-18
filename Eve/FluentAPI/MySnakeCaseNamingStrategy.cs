using Newtonsoft.Json.Serialization;

namespace Eve
{
    /// <summary>
    /// We could build our own code to avoid inheriting from a non-static class, but we want to make sure
    /// that we are using the same algorithm used by the EveClient when it is serializing models. 
    /// TODO: Once all CRUD methods are static, we might switch to our own implementation and ditch the value class.
    /// Either that, or change the class name to something more appropriate.
    /// </summary>
    class MySnakeCaseNamingStrategy : SnakeCaseNamingStrategy
    {
        public new string ResolvePropertyName(string name)
        {
            return base.ResolvePropertyName(name);
        }
    }

}
