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

    public class ReviewRefinement
    {
        public static string jarRoot = @"..\..\..\..\Users\Hp Mobile Workstatio\Documents\Visual Studio 2013\Projects\FYP DB Fix UP\FYPAdam\ReviewsAnalysis\packet-files\models";
        public static Properties props = new Properties();
        public static List<Tree> lst11 = new List<Tree>();
        public static List<string> opinionPhrases = new List<string>();
        public static DataTable positiveFeatures = new DataTable();
        public static DataTable negativeFeatures = new DataTable();

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
        public static void GetSentimentandNouns()
        {
            positiveFeatures.Columns.Add("pNouns", typeof(string));
            negativeFeatures.Columns.Add("nNouns", typeof(string));
            props.setProperty("annotators", "tokenize, ssplit, pos, parse, sentiment");
            //props.setProperty("annotators", "tokenize, ssplit, parse");
            props.setProperty("ner.useSUTime", "0");
            List<string> nouns = new List<string>();
          //  List<string> adjectives = new List<string>();
            string line = "";
            int sentiment;

            // We should change current directory, so StanfordCoreNLP could find all the model files automatically
            var curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(jarRoot);
            var pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);
            DbWrappers wrap = new DbWrappers();
            List<Product> products=wrap.GetAllReviewsAgainstProduct();
            
            foreach(var prod in products)
            {
                foreach(var review in prod.ProductReviews)
                {
                    Annotation annotation = pipeline.process(review.Review);
                    var Sentence = annotation.get(typeof(CoreAnnotations.SentencesAnnotation)) as ArrayList;
                    foreach (CoreMap sen in Sentence)
                    {
                        sentiment = GetSentiment(sen.ToString(), pipeline);
                        if (sentiment == 3 || sentiment == 4)
                        {
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

                                        if (refinedSubPharse.Contains("harddrive") || refinedSubPharse.Contains("memory") || refinedSubPharse.Contains("hdd") || refinedSubPharse.Contains("keyboard") || refinedSubPharse.Contains("processor") || refinedSubPharse.Contains("speed") || refinedSubPharse.Contains("operating system") || refinedSubPharse.Contains("display") || refinedSubPharse.Contains("graphics") || refinedSubPharse.Contains("price") || refinedSubPharse.Contains("money") || refinedSubPharse.Contains("weight") || refinedSubPharse.Contains("camera") || refinedSubPharse.Contains("dimension") || refinedSubPharse.Contains("screen") || refinedSubPharse.Contains("ram") || refinedSubPharse.Contains("hard disk") || refinedSubPharse.Contains("os") || refinedSubPharse.Contains("battery") || refinedSubPharse.Contains("performance") || refinedSubPharse.Contains("looks") || refinedSubPharse.Contains("hardware") || refinedSubPharse.Contains("speed") || refinedSubPharse.Contains("design"))
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

                                        if (refinedSubPharse.Contains("harddrive") || refinedSubPharse.Contains("memory") || refinedSubPharse.Contains("hdd") || refinedSubPharse.Contains("keyboard") || refinedSubPharse.Contains("processor") || refinedSubPharse.Contains("speed") || refinedSubPharse.Contains("operating system") || refinedSubPharse.Contains("display") || refinedSubPharse.Contains("graphics") || refinedSubPharse.Contains("price") || refinedSubPharse.Contains("money") || refinedSubPharse.Contains("weight") || refinedSubPharse.Contains("camera") || refinedSubPharse.Contains("dimension") || refinedSubPharse.Contains("screen") || refinedSubPharse.Contains("ram") || refinedSubPharse.Contains("hard disk") || refinedSubPharse.Contains("os") || refinedSubPharse.Contains("battery") || refinedSubPharse.Contains("performance") || refinedSubPharse.Contains("looks") || refinedSubPharse.Contains("hardware") || refinedSubPharse.Contains("speed") || refinedSubPharse.Contains("design") || refinedSubPharse.Contains("keys") || refinedSubPharse.Contains("ram") || refinedSubPharse.Contains("hard disk") || refinedSubPharse.Contains("os") || refinedSubPharse.Contains("battery") || refinedSubPharse.Contains("performance") || refinedSubPharse.Contains("looks") || refinedSubPharse.Contains("hardware") || refinedSubPharse.Contains("speed") || refinedSubPharse.Contains("design") || refinedSubPharse.Contains("quick"))
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

                Dictionary<string, int> featureDictionary = new Dictionary<string, int>();
                var pFeatures = from row in positiveFeatures.AsEnumerable()
                                group row by row.Field<string>("pNouns") into grp
                                select new { key = grp.Key, cnt = grp.Count() };
                foreach (var obj in pFeatures)
                {
                    if (obj.key.Equals("mouse") || obj.key.Equals("pad") || obj.key.Equals("touchpad"))
                    {
                        featureDictionary["touchpad"] += obj.cnt;
                    }
                    else
                    {
                        featureDictionary[obj.key] = obj.cnt;
                    }
                    FeatureSentiment fs = new FeatureSentiment();
                    fs.Feature = obj.key;
                    fs.PId = prod.Id;
                    fs.Sentiment = 4;
                    fs.Count = obj.cnt;
                    wrap.AddFeatureSentiment(fs);
                }

                var nFeatures = from row in negativeFeatures.AsEnumerable()
                                group row by row.Field<string>("nNouns") into grp
                                select new { key = grp.Key, cnt = grp.Count() };
                foreach (var obj in nFeatures)
                {
                    FeatureSentiment fs = new FeatureSentiment();
                    fs.Feature = obj.key;
                    fs.PId = prod.Id;
                    fs.Sentiment = 1;
                    fs.Count = obj.cnt;
                    wrap.AddFeatureSentiment(fs);
                }


            }

        }
    }
}
