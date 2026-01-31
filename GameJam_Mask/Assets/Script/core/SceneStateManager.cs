namespace Scenes {
    public class SceneStateManager {
        public Scene CurrentScene { get; set; } = Scene.InitialTrain;
        public Loop CurrentLoop { get; set; } = Loop.First;

        public enum Scene {
            InitialTrain,
            Home,
            Station,
            Train,
            End
        }

        public enum Loop {
            First,
            Second,
            Third
        }
    }
}