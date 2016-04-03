using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.io;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.trees;
using java.util;
//using java.util.Map;
using Console = System.Console;
using System.IO;
using edu.stanford.nlp.util;
using System.ComponentModel.DataAnnotations;
using edu.stanford.nlp.ling;
//using edu.stanford.nlp.pipeline.Annotation;
//using edu.stanford.nlp.pipeline.StanfordCoreNLP;
using edu.stanford.nlp.neural.rnn;
using edu.stanford.nlp.sentiment;
using edu.stanford.nlp.ie.crf;
using org.w3c.dom;
using edu.stanford.nlp.parser.lexparser;
using System.Text.RegularExpressions;
using System.Data;
using AdamDal;

namespace TextMining
{
    
   public class test
    {
        //public static string jarRoot = @"..\..\..\..\Users\Hp Mobile Workstatio\Documents\Visual Studio 2013\Projects\FYP DB Fix UP\FYPAdam\ReviewsAnalysis\packet-files\models";
       public static string jarRoot = @"../../packet-files/models";
        public static Properties props = new Properties();
        public static DataTable positiveFeatures = new DataTable();
        public static DataTable negativeFeatures = new DataTable();
        public static List<string> opinionPhrases = new List<string>();
        public static Dictionary<string, int> pfeatureDictionary = new Dictionary<string, int>();
        public static Dictionary<string, int> nfeatureDictionary = new Dictionary<string, int>();

        public static int GetSentiment(string review, dynamic pipeline)
        {


            int mainSentiment = 0;
            if (review != null && review.Length > 0)
            {
                int longest = 0;
                Annotation annotation = pipeline.process(review);

                var Sentence = annotation.get(typeof(CoreAnnotations.SentencesAnnotation)) as ArrayList;

                foreach (CoreMap sen in Sentence)
                {

                    Tree tree = (Tree)sen.get(typeof(SentimentCoreAnnotations.SentimentAnnotatedTree));



                    Console.WriteLine(tree);
                    int sentiment = RNNCoreAnnotations.getPredictedClass(tree);
                    string partText = sen.ToString();
                    if (partText.Length > longest)
                    {
                        mainSentiment = sentiment;
                        longest = partText.Length;
                    }
                }
            }

            return mainSentiment;
        }
        public static void GetSentimentandNouns(List<string> reviews)
        {
            try
            {
                positiveFeatures.Columns.Add("pNouns", typeof(string));
                negativeFeatures.Columns.Add("nNouns", typeof(string));
                props.setProperty("annotators", "tokenize, ssplit, pos, parse, sentiment");
                //props.setProperty("annotators", "tokenize, ssplit, parse");
                props.setProperty("ner.useSUTime", "0");
                List<string> nouns = new List<string>();
                List<string> adjectives = new List<string>();
                string line = "";
                int sentiment;
                int pCount = 0;// overall count of positive reviews of a product
                int nCount = 0;//overall count of negative reviews of a produt
                int overAllRating = 0;

                // We should change current directory, so StanfordCoreNLP could find all the model files automatically
                var curDir = Environment.CurrentDirectory;
                Directory.SetCurrentDirectory(jarRoot);
                var pipeline = new StanfordCoreNLP(props);
                Directory.SetCurrentDirectory(curDir);

                pfeatureDictionary.Clear();
                nfeatureDictionary.Clear();
                positiveFeatures.Clear();
                negativeFeatures.Clear();

                foreach (var review in reviews)
                {
                    Annotation annotation = pipeline.process(review);
                    var Sentence = annotation.get(typeof(CoreAnnotations.SentencesAnnotation)) as ArrayList;
                    foreach (CoreMap sen in Sentence)
                    {
                        sentiment = GetSentiment(sen.ToString(), pipeline);
                        if (sentiment == 3 || sentiment == 4)
                        {
                            pCount++;
                            java.lang.Class treeClass = new edu.stanford.nlp.trees.TreeCoreAnnotations.TreeAnnotation().getClass();
                            edu.stanford.nlp.trees.Tree tree1 = (edu.stanford.nlp.trees.Tree)sen.get(treeClass);
                            string treeString = tree1.toString();
                            MatchCollection mc = Regex.Matches(treeString, @"(([N][N]\s\w*)|([N][N][P]\s\w*) |([N][P]\s\w*))");
                            foreach (Match m in mc)
                            {
                                string opinionPhrase = "";
                                string opinionString = m.ToString();
                                var phrase = opinionString.Split(' ');
                                foreach (var subPhrase in phrase)
                                {
                                    if (!(subPhrase.Contains("NP")) && !(subPhrase.Contains("NNP")) && !(subPhrase.Contains("NNS")) && !(subPhrase.Contains("NN")))
                                    {
                                        String refinedSubPharse1 = subPhrase.Replace(")", string.Empty);
                                        //Console.WriteLine(refinedSubPharse1);
                                        string refinedSubPharse = refinedSubPharse1.ToLower();

                                        if (refinedSubPharse.Equals("harddrive") || refinedSubPharse.Equals("memory") || refinedSubPharse.Equals("hdd") || refinedSubPharse.Equals("keyboard") || refinedSubPharse.Equals("processor") || refinedSubPharse.Equals("operating system") || refinedSubPharse.Equals("display") || refinedSubPharse.Equals("graphics") || refinedSubPharse.Equals("price") || refinedSubPharse.Equals("money") || refinedSubPharse.Equals("weight") || refinedSubPharse.Equals("camera") || refinedSubPharse.Equals("dimension") || refinedSubPharse.Equals("screen") || refinedSubPharse.Equals("ram") || refinedSubPharse.Equals("hard disk") || refinedSubPharse.Equals("os") || refinedSubPharse.Equals("battery") || refinedSubPharse.Equals("performance") || refinedSubPharse.Equals("looks") || refinedSubPharse.Equals("hardware") || refinedSubPharse.Equals("speed") || refinedSubPharse.Equals("design") || refinedSubPharse.Equals("size") || refinedSubPharse.Equals("boot") || refinedSubPharse.Equals("boots") || refinedSubPharse.Equals("drive") || refinedSubPharse.Equals("mouse") || refinedSubPharse.Equals("response") || refinedSubPharse.Equals("quick") || refinedSubPharse.Equals("sound") || refinedSubPharse.Equals("cam") || refinedSubPharse.Equals("picture"))
                                        {
                                            opinionPhrases.Add(refinedSubPharse);
                                            DataRow newRow = positiveFeatures.NewRow();
                                            newRow["pNouns"] = refinedSubPharse.ToString();
                                            positiveFeatures.Rows.Add(newRow);
                                        }
                                        //opinionPhrase += refinedSubPharse + " ";
                                    }
                                }


                            }
                        }//end sentiment if

                        else if (sentiment == 1 || sentiment == 0)
                        {
                            nCount++;
                            java.lang.Class treeClass = new edu.stanford.nlp.trees.TreeCoreAnnotations.TreeAnnotation().getClass();
                            edu.stanford.nlp.trees.Tree tree1 = (edu.stanford.nlp.trees.Tree)sen.get(treeClass);
                            string treeString = tree1.toString();
                            MatchCollection mc = Regex.Matches(treeString, @"(([N][N]\s\w*)|([N][N][P]\s\w*) |([N][P]\s\w*))");
                            foreach (Match m in mc)
                            {
                                string opinionPhrase = "";
                                string opinionString = m.ToString();
                                var phrase = opinionString.Split(' ');
                                foreach (var subPhrase in phrase)
                                {
                                    if (!(subPhrase.Contains("NP")) && !(subPhrase.Contains("NNP")) && !(subPhrase.Contains("NNS")) && !(subPhrase.Contains("NN")))
                                    {
                                        String refinedSubPharse1 = subPhrase.Replace(")", string.Empty);
                                        //Console.WriteLine(refinedSubPharse1);
                                        string refinedSubPharse = refinedSubPharse1.ToLower();

                                        if (refinedSubPharse.Equals("harddrive") || refinedSubPharse.Equals("memory") || refinedSubPharse.Equals("hdd") || refinedSubPharse.Equals("keyboard") || refinedSubPharse.Equals("processor") || refinedSubPharse.Equals("operating system") || refinedSubPharse.Equals("display") || refinedSubPharse.Equals("graphics") || refinedSubPharse.Equals("price") || refinedSubPharse.Equals("money") || refinedSubPharse.Equals("weight") || refinedSubPharse.Equals("camera") || refinedSubPharse.Equals("dimension") || refinedSubPharse.Equals("screen") || refinedSubPharse.Equals("ram") || refinedSubPharse.Equals("hard disk") || refinedSubPharse.Equals("os") || refinedSubPharse.Equals("battery") || refinedSubPharse.Equals("performance") || refinedSubPharse.Equals("looks") || refinedSubPharse.Equals("hardware") || refinedSubPharse.Equals("speed") || refinedSubPharse.Equals("design") || refinedSubPharse.Equals("size") || refinedSubPharse.Equals("boot") || refinedSubPharse.Equals("boots") || refinedSubPharse.Equals("drive") || refinedSubPharse.Equals("mouse") || refinedSubPharse.Equals("response") || refinedSubPharse.Equals("quick") || refinedSubPharse.Equals("sound") || refinedSubPharse.Equals("cam") || refinedSubPharse.Equals("picture"))
                                        {
                                            opinionPhrases.Add(refinedSubPharse);
                                            DataRow newRow = negativeFeatures.NewRow();
                                            newRow["nNouns"] = refinedSubPharse.ToString();
                                            negativeFeatures.Rows.Add(newRow);
                                        }
                                        //opinionPhrase += refinedSubPharse + " ";
                                    }
                                }


                            }
                        }//end sentiment if
                    }
                }//end of review loop


                bool checkTouchpad = false;
                // bool checkKeyboard = false;
                bool checkPrice = false;
                bool checkHarddrive = false;
                bool checkMemory = false;
                bool checkDisplay = false;
                bool checkOs = false;
                bool checkProcessor = false;
                bool checkPerformance = false;
                bool checkCamera = false;
                var pFeatures = from row in positiveFeatures.AsEnumerable()
                                group row by row.Field<string>("pNouns") into grp
                                select new { key = grp.Key, cnt = grp.Count() };
                foreach (var obj in pFeatures)
                {
                    if (obj.key.Equals("mouse") || obj.key.Equals("pad") || obj.key.Equals("touchpad"))
                    {
                        if (checkTouchpad)
                        {
                            pfeatureDictionary["Touchpad"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Touchpad"] = obj.cnt;
                            checkTouchpad = true;
                        }
                    }


                    else if (obj.key.Equals("camera") || obj.key.Equals("cam") || obj.key.Equals("picture"))
                    {
                        if (checkCamera)
                        {
                            pfeatureDictionary["Camera"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Camera"] = obj.cnt;
                            checkCamera = true;
                        }
                    }
                    else if (obj.key.Equals("money") || obj.key.Equals("price"))
                    {
                        if (checkPrice)
                        {
                            pfeatureDictionary["Price"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Price"] = obj.cnt;
                            checkPrice = true;
                        }
                    }



                    else if (obj.key.Equals("hdd") || obj.key.Equals("harddrive") || obj.key.Equals("cam") || obj.key.Equals("picture"))
                    {
                        if (checkHarddrive)
                        {
                            pfeatureDictionary["Harddrive"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Harddrive"] = obj.cnt;
                            checkHarddrive = true;
                        }
                    }



                    else if (obj.key.Equals("graphics") || obj.key.Equals("display") || obj.key.Equals("screen"))
                    {
                        if (checkDisplay)
                        {
                            pfeatureDictionary["Screen"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Screen"] = obj.cnt;
                            checkDisplay = true;
                        }
                    }



                    else if (obj.key.Equals("ram") || obj.key.Equals("memory"))
                    {
                        if (checkMemory)
                        {
                            pfeatureDictionary["Ram"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Ram"] = obj.cnt;
                            checkMemory = true;
                        }
                    }


                    else if (obj.key.Equals("os") || obj.key.Equals("operatingsystem"))
                    {
                        if (checkOs)
                        {
                            pfeatureDictionary["OS"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Operating System"] = obj.cnt;
                            checkOs = true;
                        }
                    }

                    else if (obj.key.Equals("speed") || obj.key.Equals("processor") || obj.key.Equals("response"))
                    {
                        if (checkProcessor)
                        {
                            pfeatureDictionary["Processor"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Processor"] = obj.cnt;
                            checkProcessor = true;
                        }
                    }

                    else if (obj.key.Equals("performance") || obj.key.Equals("boot") || obj.key.Equals("boots") || obj.key.Equals("response"))
                    {
                        if (checkPerformance)
                        {
                            pfeatureDictionary["Performance"] += obj.cnt;
                        }
                        else
                        {
                            pfeatureDictionary["Performance"] = obj.cnt;
                            checkPerformance = true;
                        }
                    }
                    else
                    {
                        pfeatureDictionary[obj.key] = obj.cnt;
                    }



                }

                checkTouchpad = false;
                // bool checkKeyboard = false;
                checkPrice = false;
                checkHarddrive = false;
                checkMemory = false;
                checkDisplay = false;
                checkOs = false;
                checkProcessor = false;
                checkPerformance = false;
                checkCamera = false;

                var nFeatures = from row in negativeFeatures.AsEnumerable()
                                group row by row.Field<string>("nNouns") into grp
                                select new { key = grp.Key, cnt = grp.Count() };
                foreach (var obj in nFeatures)
                {
                    if (obj.key.Equals("mouse") || obj.key.Equals("pad") || obj.key.Equals("touchpad"))
                    {
                        if (checkTouchpad)
                        {
                            nfeatureDictionary["Touchpad"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Touchpad"] = obj.cnt;
                            checkTouchpad = true;
                        }
                    }

                    else if (obj.key.Equals("camera") || obj.key.Equals("cam") || obj.key.Equals("picture"))
                    {
                        if (checkCamera)
                        {
                            nfeatureDictionary["Camera"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Camera"] = obj.cnt;
                            checkCamera = true;
                        }
                    }

                    else if (obj.key.Equals("money") || obj.key.Equals("price"))
                    {
                        if (checkPrice)
                        {
                            nfeatureDictionary["Price"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Price"] = obj.cnt;
                            checkPrice = true;
                        }
                    }



                    else if (obj.key.Equals("hdd") || obj.key.Equals("harddrive"))
                    {
                        if (checkHarddrive)
                        {
                            nfeatureDictionary["Hard Drive"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Hard Drive"] = obj.cnt;
                            checkHarddrive = true;
                        }
                    }



                    else if (obj.key.Equals("graphics") || obj.key.Equals("display") || obj.key.Equals("screen"))
                    {
                        if (checkDisplay)
                        {
                            nfeatureDictionary["Screen"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Screen"] = obj.cnt;
                            checkDisplay = true;
                        }
                    }

                                //


                    else if (obj.key.Equals("ram") || obj.key.Equals("memory"))
                    {
                        if (checkMemory)
                        {
                            nfeatureDictionary["Ram"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Ram"] = obj.cnt;
                            checkMemory = true;
                        }
                    }



                    else if (obj.key.Equals("os") || obj.key.Equals("operatingsystem"))
                    {
                        if (checkOs)
                        {
                            nfeatureDictionary["Operating System"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Operating System"] = obj.cnt;
                            checkOs = true;
                        }
                    }

                    else if (obj.key.Equals("speed") || obj.key.Equals("processor") || obj.key.Equals("response"))
                    {
                        if (checkProcessor)
                        {
                            nfeatureDictionary["Processor"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Processor"] = obj.cnt;
                            checkProcessor = true;
                        }
                    }

                    else if (obj.key.Equals("performance") || obj.key.Equals("boot") || obj.key.Equals("boots") || obj.key.Equals("response"))
                    {
                        if (checkPerformance)
                        {
                            nfeatureDictionary["Performance"] += obj.cnt;
                        }
                        else
                        {
                            nfeatureDictionary["Performance"] = obj.cnt;
                            checkPerformance = true;
                        }
                    }
                    else
                    {
                        nfeatureDictionary[obj.key] = obj.cnt;
                    }

                }

                //checking the frequency of features to get the predict positive and negative ones
                foreach (var obj in pfeatureDictionary)
                {
                    if (nfeatureDictionary.ContainsKey(obj.Key))
                    {
                        if (pfeatureDictionary[obj.Key] > nfeatureDictionary[obj.Key])
                        {
                            nfeatureDictionary.Remove(obj.Key);
                        }
                    }
                }

                foreach (var obj in nfeatureDictionary)
                {
                    if (pfeatureDictionary.ContainsKey(obj.Key))
                    {
                        if (pfeatureDictionary[obj.Key] < nfeatureDictionary[obj.Key])
                        {
                            pfeatureDictionary.Remove(obj.Key);
                        }
                    }
                }
                foreach (var obj in pfeatureDictionary)
                {
                    //FeatureSentiment fs = new FeatureSentiment();
                    //fs.Feature = obj.Key;
                    //fs.PId = prod.Id;
                    //fs.Sentiment = 4;
                    //fs.Count = obj.Value;
                    //wrap.AddFeatureSentiment(fs);
                }

                foreach (var obj in nfeatureDictionary)
                {

                    //FeatureSentiment fs = new FeatureSentiment();
                    //fs.Feature = obj.Key;
                    //fs.PId = prod.Id;
                    //fs.Sentiment = 1;
                    //fs.Count = obj.Value;
                    //wrap.AddFeatureSentiment(fs);
                }

                if (pCount > nCount)
                {
                    overAllRating = pCount;
                }
                else
                {
                    overAllRating = nCount;
                }

            }catch (Exception)
                {
                    
                }
            finally
            {
                positiveFeatures.Columns.Remove("pNouns");
                negativeFeatures.Columns.Remove("nNouns");
            }
        }
    }
}
