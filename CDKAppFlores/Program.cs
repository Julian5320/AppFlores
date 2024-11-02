using Amazon.CDK;

namespace CDKAppFlores
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new CDK.ProyectoStack(app, "CDKId", new StackProps
            {
            });
            app.Synth();
        }
    }
}