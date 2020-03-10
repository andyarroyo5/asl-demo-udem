using Microsoft.CognitiveServices.Speech;
using MLPlaygroundML.Model;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace asl_demo
{
    class Program
    {
        //Rutas a nuestro modelo e img de prueba
        private static string TestDatasetsRelativePath = @"../../../../Data/Test/Random";
        private static string BaseModelsRelativePath = @"../../../../MLPlaygroundML.Model";
        private static string ModelRelativePath = $"{BaseModelsRelativePath}/MLModel.zip";

        static async Task Main(string[] args)
        {
            //utilizar clase ModelInput para cargar los datos a probar
            var input = new ModelInput();

            var filepath = TestDatasetsRelativePath;
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.jpg"))
            {
                //Especificar el nombre de la img para predecir
                input.ImageSource = filepath + @"\" + file.Name;
                Console.WriteLine("Testing" + filepath);

                //Predecir img usando la clase ConsumeModel
                ModelOutput result = ConsumeModel.Predict(input);
                Console.WriteLine($"Prediction: {result.Prediction}\n Score: {result.Score[0]}");


                //Checar que el resultado sea correcto. El nombre de la img de prueba es: el valor correcto + número de iteración
                var label = Regex.Replace(file.Name, @"[\d-]", string.Empty);
                label = Regex.Replace(label, @".jpg", string.Empty);
                Console.WriteLine($"Actual (Real) Label:" + label);
                if (label.Equals(result.Prediction))
                {
                    //Utilizar cognitive services para usar text 2 speech
                    await SynthesisToSpeakerAsync(result.Prediction);
                    Console.WriteLine("Correct");
                }
                else
                    Console.WriteLine("Incorrect");
            }
            Console.WriteLine("Done");
            Console.WriteLine("Ejemplo Speech2text?");
            var respuesta = Console.ReadLine();
            if (respuesta.Equals("y"))
            {
                var texto = "Impossible, only means that you haven’t found the solution; yet";
                await SynthesisToSpeakerAsync(texto);
                Console.WriteLine(texto);
            }

        }

        public static async Task SynthesisToSpeakerAsync(string text)
        {

            //Se crea una instancia de speech config 
            //FromSubscription(key, region de servicio) (e.g., southus, westus) se obtiene en portal sección Quickstart            
            var config = SpeechConfig.FromSubscription("82f7eceec77143e59953f085a8c7addd", "southcentralus");

            // Crea un sintetizador de speech usando el speaker default como output de audio
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                // text: resultado de nuestra predicción del modelo 

                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized to speaker for text [{text}]");
                    }

                    //Manejar error o cancelación del resultado 
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
    }
}

