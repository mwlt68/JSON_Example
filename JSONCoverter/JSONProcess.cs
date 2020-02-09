using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONCoverter
{
    public class JSONProcess
    {
        public static bool Write(string filePath,List<Question> questions )
        {
            try
            {
                string questionsStr = Newtonsoft.Json.JsonConvert.SerializeObject(questions);
                File.WriteAllText(filePath, questionsStr);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("File Write error !");
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public static List<Question> Read(string filePath)
        {
            try
            {
                string questionsStr = File.ReadAllText(filePath);
                List<Question> questions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Question>>(questionsStr);
                return questions;
            }
            catch (Exception e)
            {
                Console.WriteLine("File Read error !");
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public static void backup(string filePath,List<Question> questions)
        {
            try
            {
                string questionsStr = Newtonsoft.Json.JsonConvert.SerializeObject(questions);
                File.WriteAllText(filePath, questionsStr);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
