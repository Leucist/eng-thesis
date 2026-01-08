namespace Application.AI
{
    public class TrainingDataGenerator
    {
        public static TrainingData[] GenerateTrainingData()
        {
            var data = new List<TrainingData>();

            // // STRATEGY 1: AI won with balanced distribution -> keep it
            // for (int i = 0; i < 50; i++)
            // {
            //     float 
            //     data.Add(new TrainingData
            //     {
            //         Input   = [0.33f + Rand(0.15f), 0.33f + Rand(0.15f), 0.33f + Rand(0.15f), 1f],
            //         Target  = [0f, 0f, 0f, 1f] // no change
            //     });
            // }

            // STRATEGY 2: AI lost with too many close enemies -> push farther
            for (int i = 0; i < 80; i++)
            {
                float closePct = 0.6f + Rand(0.15f);
                float midPct = Rand(0.15f);
                float farPct = 1f - closePct - midPct;

                data.Add(new TrainingData
                {
                    Input   = [closePct, midPct, farPct, 0f],
                    Target  = [0.05f, 0.5f, 0.4f, 0.05f] // reinforce mid or far
                });
            }

            // STRATEGY 3: AI lost with all far enemies -> bring them closer
            for (int i = 0; i < 80; i++)
            {
                float farPct = 0.7f + Rand(0.2f);
                float midPct = Rand(0.1f);
                float closePct = 1f - farPct - midPct;

                data.Add(new TrainingData
                {
                    Input   = [closePct, midPct, farPct, 0f],
                    Target  = [0.6f, 0.3f, 0.05f, 0.05f] // reinforce close
                });
            }

            // STRATEGY 4: AI won with aggressive close formation -> keep it
            for (int i = 0; i < 60; i++)
            {
                float closePct = 0.65f + Rand(0.2f);
                float midPct = Rand(0.2f);
                float farPct = 1f - closePct - midPct;

                data.Add(new TrainingData
                {
                    Input = [closePct, midPct, farPct, 1f],
                    Target = [0f, 0f, 0f, 1f] // noChange - it worked!
                });
            }

            // STRATEGY 5: AI lost with too spread out -> consolidate to mid
            for (int i = 0; i < 70; i++)
            {
                float closePct = 0.2f + Rand(0.2f);
                float farPct = 0.2f + Rand(0.2f);
                float midPct = 1f - closePct - farPct;

                data.Add(new TrainingData
                {
                    Input   = [closePct, midPct, farPct, 1f],
                    Target  = [0.2f, 0.6f, 0.2f, 0f] // reinforceMid - group up!
                });
            }

            // STRATEGY 6: AI won with mid-range dominance -> keep it
            for (int i = 0; i < 50; i++)
            {
                float midPct = 0.6f + Rand(0.2f);
                float closePct = Rand(0.2f);
                float farPct = 1f - midPct - closePct;

                data.Add(new TrainingData
                {
                    Input   = [closePct, midPct, farPct, 1f],
                    Target  = [0f, 0f, 0f, 1f] // noChange
                });
            }

            // STRATEGY 7: AI lost with only mid enemies -> try extremes
            for (int i = 0; i < 60; i++)
            {
                float midPct = 0.8f + Rand(0.15f);
                float closePct = Rand(0.1f);
                float farPct = 1f - midPct - closePct;

                data.Add(new TrainingData
                {
                    Input   = [closePct, midPct, farPct, 0f],
                    Target  = [0.45f, 0.1f, 0.45f, 0f] // try close or far
                });
            }

            // STRATEGY 8: AI won with spread out distr -> keep
            for (int i = 0; i < 70; i++)
            {
                float closePct = 0.2f + Rand(0.1f);
                float midPct = 0.2f + Rand(0.1f);
                float farPct = 1f - closePct - midPct;

                data.Add(new TrainingData
                {
                    Input   = [closePct, midPct, farPct, 1f],
                    Target  = [0f, 0f, 0f, 1f] // no change
                });
            }

            // STRATEGY 9: AI lost with no close pressure -> bring closer
            for (int i = 0; i < 90; i++)
            {
                float closePct = Rand(0.15f);
                float midPct = 0.4f + Rand(0.2f);
                float farPct = 1f - closePct - midPct;

                data.Add(new TrainingData
                {
                    Input   = [closePct, midPct, farPct, 0f],
                    Target  = [0.8f, 0.15f, 0.05f, 0f] // reinforce close strongly
                });
            }

            return data.ToArray();
        }

        private static Random rand = new Random();
        private static float Rand(float range) => (float)(rand.NextDouble() * range - range / 2);
    }
}