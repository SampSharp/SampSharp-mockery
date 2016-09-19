namespace SampSharp.Mockery
{
    public class FakePlayerDefinition : FakeNativeObjectDefinition
    {
        public FakePlayerDefinition(ServerImitator server) : base(server)
        {
        }

        public override void Define()
        {
            ProvidesNative("SetPlayerPosition", 0, FakeNativeReturnBehavior.HandleExists, new[]
            {
                new FakeNativeValue("id", ParameterType.Int),
                new FakeNativeValue("posx", ParameterType.Float),
                new FakeNativeValue("posy", ParameterType.Float),
                new FakeNativeValue("posz", ParameterType.Float)
            });

            ProvidesNative("GetPlayerPosition", 0, FakeNativeReturnBehavior.HandleExists, new[]
            {
                new FakeNativeValue("id", ParameterType.Int),
                new FakeNativeValue("posx", ParameterType.Float.ByRef()),
                new FakeNativeValue("posy", ParameterType.Float.ByRef()),
                new FakeNativeValue("posz", ParameterType.Float.ByRef())
            });

            ProvidesNative("SetPlayerName", 0, FakeNativeReturnBehavior.HandleExists, new[]
            {
                new FakeNativeValue("id", ParameterType.Int),
                new FakeNativeValue("name", ParameterType.String)
            });

            ProvidesNative("GetPlayerName", 0, FakeNativeReturnBehavior.HandleExists, new[]
            {
                new FakeNativeValue("id", ParameterType.Int),
                new FakeNativeValue("name", ParameterType.String.ByRef())
            });

            // TODO:
//            ProvidesGetterNative("GetPlayerPing", ParameterType.Int, "ping");

        }
    }
}