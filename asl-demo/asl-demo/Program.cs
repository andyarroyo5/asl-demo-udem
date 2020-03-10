using MLPlaygroundML.Model;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace asl_demo
{
    class Program
    {
        //Rutas a nuestro modelo e img de prueba
        private static string TestDatasetsRelativePath = @"../../../../Data/Test/Random";
        private static string BaseModelsRelativePath = @"../../../../MLPlaygroundML.Model";
        private static string ModelRelativePath = $"{BaseModelsRelativePath}/MLModel.zip";
        static void Main(string[] args)
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
                    Console.WriteLine("Correct");
                }
                else
                    Console.WriteLine("Incorrect");
            }
            Console.WriteLine("Done");
            Console.WriteLine("Ejemplo Speech2text?");
            var respuesta = Console.ReadLine();            

        }
    }
    }
}
