using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using AdaptiveExpressions.Properties;
using System.Diagnostics;
using System;

namespace AdaptiveCards.Templating.Test
{
    [TestClass]
    public class TestTemplate
    {
        [TestMethod]
        public void TestBasic()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""$data"": {
                ""person"": {
                    ""firstName"": ""Andrew"",
                    ""lastName"": ""Leader""
                }
     },
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""Hello ${person.firstName}""
        }
    ]
}";

            string jsonData = @"{
    ""person"": {
        ""firstName"": ""Andrew"",
        ""lastName"": ""Leader""
    }
}";

            string cardJson = transformer.Transform(jsonTemplate, jsonData);

            AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""Hello Andrew""
        }
    ]
}", cardJson);
        }

        [TestMethod]
        public void TestExternalDataContext()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""Hello ${person.firstName}""
        }
    ]
}";

            string jsonData = @"{
    ""person"": {
        ""firstName"": ""Andrew"",
        ""lastName"": ""Leader""
    }
}";

            string cardJson = transformer.Transform(jsonTemplate, jsonData);

            AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""Hello Andrew""
        }
    ]
}", cardJson);
        }

        [TestMethod]
        public void TestExternalDataContextInternalReference()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
            ""type"": ""AdaptiveCard"",
            ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                  {
                    ""$data"": ""${LineItems}"",
                    ""type"": ""TextBlock"",
                    ""$when"": ""${Milage > 0}"",
                    ""text"": ""${Milage}""
                  }
                ]
              }
            ]
        }";

            string jsonData = @"{
              ""LineItems"": [
                {
                    ""Milage"": 10
                },
                {
                    ""Milage"": 0
                }
              ]
            }";

            string cardJson = transformer.Transform(jsonTemplate, jsonData);

            AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                {
                    ""type"": ""TextBlock"",
                    ""text"": ""10""
                }
            ]
        }
    ]
}", cardJson);
        }

        public static void AssertJsonEqual(string jsonExpected, string jsonActual)
        {
            var expected = JObject.Parse(jsonExpected);
            var actual = JObject.Parse(jsonActual);

            Assert.IsTrue(JToken.DeepEquals(expected, actual), "JSON wasn't the same.\n\nExpected: " + expected.ToString() + "\n\nActual: " + actual.ToString());
        }

        [TestMethod]
        public void TestArray()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var testString =
                @"{
                ""type"": ""AdaptiveCard"",
                ""$data"": {
                    ""employee"": {
                        ""name"": ""Matt"",
                        ""manager"": { ""name"": ""Thomas"" },
                        ""peers"": [{
                            ""name"": ""Andrew"" 
                        }, { 
                            ""name"": ""Lei""
                        }, { 
                            ""name"": ""Mary Anne""
                        }, { 
                            ""name"": ""Adam""
                        }]
                    }
                },
                ""body"": [
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Hi ${employee.name}! Here's a bit about your org...""
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Your manager is: ${employee.manager.name}""
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""3 of your peers are: ${employee.peers[0].name}, ${employee.peers[1].name}, ${employee.peers[2].name}""
                    }
                ]
            }";

            var expectedString = @"{
                ""type"": ""AdaptiveCard"",
                ""body"": [
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Hi Matt! Here's a bit about your org...""
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""Your manager is: Thomas""
                    },
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""3 of your peers are: Andrew, Lei, Mary Anne""
                    }
                ]
            }";

            string cardJson = transformer.Transform(testString, null);
            AssertJsonEqual(expectedString, cardJson);
        }

        [TestMethod]
        public void TestIteratioinWithFacts()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var testString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""version"": ""1.0"",
                    ""body"": [
                        {
                            ""type"": ""Container"",
                            ""items"": [
                                {
                                    ""type"": ""TextBlock"",
                                    ""text"": ""Header""
                                },
                                {
                                    ""type"": ""FactSet"",
                                    ""facts"": [
                                        {
                                            ""$data"": [
                                                {
                                                    ""name"": ""Star"",
                                                    ""nickname"": ""Dust""
                                                },
                                                {
                                                    ""name"": ""Death"",
                                                    ""nickname"": ""Star""
                                                }
                                            ],
                                            ""title"": ""${name}"",
                                            ""value"": ""${nickname}""
                                        }
                                    ]
                                }
                            ]
                        }
                    ],
                    ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json""
}";
            var expectedString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""version"": ""1.0"",
                    ""body"": [
                        {
                            ""type"": ""Container"",
                            ""items"": [
                                {
                                    ""type"": ""TextBlock"",
                                    ""text"": ""Header""
                                },
                                {
                                    ""type"": ""FactSet"",
                                    ""facts"": [
                                        {
                                            ""title"": ""Star"",
                                            ""value"": ""Dust""
                                        },
                                        {
                                            ""title"": ""Death"",
                                            ""value"": ""Star""
                                        }
                                    ]
                                }
                            ]
                        }
                    ],
                    ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json""
}";

            string cardJson = transformer.Transform(testString, null);
            AssertJsonEqual(expectedString, cardJson);
        }

        [TestMethod]
        public void TestIteratioin()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var testString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""body"": [
                      {
                          ""type"": ""Container"",
                          ""items"": [
                              {
                                  ""type"": ""TextBlock"",
                                  ""$data"": [
                                      { ""name"": ""Matt"" }, 
                                      { ""name"": ""David"" }, 
                                      { ""name"": ""Thomas"" }
                                  ],
                                  ""text"": ""${name}""
                              }
                          ]
                      }
                    ]
                }";
            var expectedString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""body"": [
                    {
                        ""type"": ""Container"",
                        ""items"": [ 
                            {
                                ""type"": ""TextBlock"",
                                ""text"": ""Matt""
                            },
                            {
                                ""type"": ""TextBlock"",
                                ""text"": ""David""
                            },
                            {
                                ""type"": ""TextBlock"",
                                ""text"": ""Thomas""
                            }
                        ]
                    }
                ]
            }";

            string cardJson = transformer.Transform(testString, null);
            AssertJsonEqual(expectedString, cardJson);
        }

        [TestMethod]
        public void TestIteratioinFalsePositive()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var testString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""body"": [
                      {
                          ""type"": ""Container"",
                          ""items"": [
                              {
                                  ""type"": ""TextBlock"",
                                  ""$data"": [
                                      { ""name"": ""Matt"" }, 
                                      { ""name"": ""David"" }, 
                                      { ""name"": ""Thomas"" }
                                  ],
                                  ""text"": ""Hello World!""
                              }
                          ]
                      }
                    ]
                }";
            var expectedString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""body"": [
                    {
                        ""type"": ""Container"",
                        ""items"": [ 
                            {
                                ""type"": ""TextBlock"",
                                ""text"": ""Hello World!""
                            }
                        ]
                    }
                ]
            }";

            string cardJson = transformer.Transform(testString, null);
            AssertJsonEqual(expectedString, cardJson);
        }

        [TestMethod]
        public void TestIteratioinRealDdata()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var templateData =
                @" [
                        { ""name"": ""Object-1"", ""lastPrice"": 1.10762, ""changePriceRatio"": -0.17 },
                        { ""name"": ""Object-2"", ""lastPrice"": 1578.205, ""changePriceRatio"": -0.68 },
                        { ""name"": ""Object-3"", ""lastPrice"": 51.475, ""changePriceRatio"": -0.23 },
                        { ""name"": ""Object-4"", ""lastPrice"": 28324, ""changePriceRatio"": 0.35 },
                        { ""name"": ""Object-5"", ""lastPrice"": 9338.87, ""changePriceRatio"": -1.04 }
                    ]";
                var testString =
                    @"{
        ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
        ""type"": ""AdaptiveCard"",
        ""version"": ""1.0"",
        ""body"": [
        {
            ""type"": ""Container"",
            ""items"": [
                {
                    ""type"": ""ColumnSet"",
                    ""columns"": [
                        {
                            ""type"": ""Column"",
                            ""width"": ""auto"",
                            ""items"": [
                                {
                                    ""type"": ""TextBlock"",
                                    ""text"": ""${if(changePriceRatio >= 0, '▲', '▼')}"",
                                    ""color"": ""${if(changePriceRatio >= 0, 'good', 'attention')}""
                                }
                            ]
                        },
                        {
                            ""type"": ""Column"",
                            ""width"": ""stretch"",
                            ""items"": [
                                {
                                    ""type"": ""TextBlock"",
                                    ""text"": ""${name}""
                                } ]
                        },
                        {
                            ""type"": ""Column"",
                            ""width"": ""stretch"",
                            ""items"": [
                                {
                                    ""type"": ""TextBlock"",
                                    ""text"": ""${lastPrice} "",
                                    ""horizontalAlignment"": ""Center""
                                }
                            ],
                            ""horizontalAlignment"": ""Center""
                        },
                        {
                            ""type"": ""Column"",
                            ""width"": ""auto"",
                            ""items"": [
                                {
                                    ""type"": ""TextBlock"",
                                    ""text"": ""${changePriceRatio}%"",
                                    ""color"": ""${if(changePriceRatio >= 0, 'good', 'attention')}""
                                }
                            ],
                            ""horizontalAlignment"": ""Right""
                        }
                    ]
                }
            ] ,
            ""$data"":""{$root}""
        }
    ]
}";
        var expectedString =
        @"{ ""$schema"":""http://adaptivecards.io/schemas/adaptive-card.json"",""type"":""AdaptiveCard"",""version"":""1.0"",""body"":[{""type"":""Container"",""items"":[{""type"":""ColumnSet"",""columns"":[{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""▼"",""color"":""attention""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""Object-1""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""1.10762 "",""horizontalAlignment"":""Center""}],""horizontalAlignment"":""Center""},{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""-0.17%"",""color"":""attention""}],""horizontalAlignment"":""Right""}]}]},{""type"":""Container"",""items"":[{""type"":""ColumnSet"",""columns"":[{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""▼"",""color"":""attention""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""Object-2""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""1578.205 "",""horizontalAlignment"":""Center""}],""horizontalAlignment"":""Center""},{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""-0.68%"",""color"":""attention""}],""horizontalAlignment"":""Right""}]}]},{""type"":""Container"",""items"":[{""type"":""ColumnSet"",""columns"":[{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""▼"",""color"":""attention""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""Object-3""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""51.475 "",""horizontalAlignment"":""Center""}],""horizontalAlignment"":""Center""},{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""-0.23%"",""color"":""attention""}],""horizontalAlignment"":""Right""}]}]},{""type"":""Container"",""items"":[{""type"":""ColumnSet"",""columns"":[{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""▲"",""color"":""good""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""Object-4""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""28324 "",""horizontalAlignment"":""Center""}],""horizontalAlignment"":""Center""},{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""0.35%"",""color"":""good""}],""horizontalAlignment"":""Right""}]}]},{""type"":""Container"",""items"":[{""type"":""ColumnSet"",""columns"":[{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""▼"",""color"":""attention""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""Object-5""}]},{""type"":""Column"",""width"":""stretch"",""items"":[{""type"":""TextBlock"",""text"":""9338.87 "",""horizontalAlignment"":""Center""}],""horizontalAlignment"":""Center""},{""type"":""Column"",""width"":""auto"",""items"":[{""type"":""TextBlock"",""text"":""-1.04%"",""color"":""attention""}],""horizontalAlignment"":""Right""}]}]}]}"; 
            string cardJson = transformer.Transform(testString, templateData);
            AssertJsonEqual(expectedString, cardJson);
        }

        [TestMethod]
        public void TestExpression()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""$data"": {
                ""machines"": {
                    ""id"": ""primary"",
                    ""uptime"": 2231
                }
     },
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""${machines.id}"",
            ""color"": ""${if(machines.uptime >= 3000, 'good', 'attention')}""
        }
    ]
}";

            string cardJson = transformer.Transform(jsonTemplate, null);

            AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""primary"",
            ""color"": ""attention""
        }
    ]
}", cardJson);
        }

        [TestMethod]
        public void TestWhen()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""$data"": {
                ""machines"": {
                    ""id"": ""primary"",
                    ""uptime"": 3231
                }
     },
    ""body"": [
        {
            ""$when"": ""${(machines.uptime >= 3000)}"",
            ""type"": ""TextBlock"",
            ""text"": ""${machines.id}""
        }
    ]
}";

            string cardJson = transformer.Transform(jsonTemplate, null);

            AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
        {
            ""type"": ""TextBlock"",
            ""text"": ""primary""
        }
    ]
}", cardJson);
        }
        [TestMethod]
        public void TestWhenWithArray()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
            ""type"": ""AdaptiveCard"",
            ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                  {
                    ""$data"": ""${LineItems}"",
                    ""type"": ""TextBlock"",
                    ""$when"": ""${Milage > 0}"",
                    ""text"": ""${Milage}""
                  }
                ]
              }
            ]
        }";

            string jsonData = @"{
              ""LineItems"": [
                    {""Milage"" : 1},
                    {""Milage"" : 10}
                ]
            }";

            string cardJson = transformer.Transform(jsonTemplate, jsonData);

            AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                {
                    ""type"": ""TextBlock"",
                    ""text"": ""1""
                },
                {
                    ""type"": ""TextBlock"",
                    ""text"": ""10""
                }
            ]
        }
    ]
}", cardJson);
        }

        [TestMethod]
        public void TestIndex()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
            ""type"": ""AdaptiveCard"",
            ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                  {
                    ""$data"": [{""Milage"" : 1}, {""Milage"" : 10}],
                    ""type"": ""TextBlock"",
                    ""id"": ""ReceiptRequired${$index}"",
                    ""text"": ""${Milage}""
                  }
                ]
              }
            ]
        }";

            string cardJson = transformer.Transform(jsonTemplate, null);

            AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                {
                    ""type"": ""TextBlock"",
                    ""id"": ""ReceiptRequired0"",
                    ""text"": ""1""
                },
                {
                    ""type"": ""TextBlock"",
                    ""id"": ""ReceiptRequired1"",
                    ""text"": ""10""
                }
            ]
        }
    ]
}", cardJson);
        }

        [TestMethod]
        public void TestInlineMemoryScope()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var testString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""version"": ""1.0"",
                    ""body"": [
                      {
                          ""type"": ""Container"",
                          ""items"": [
                              {
                                  ""type"": ""TextBlock"",
                                  ""$data"": { ""name"": ""Matt"" }, 
                                  ""text"": ""${name}""
                              }
                          ]
                      }
                    ]
                }";
            var expectedString =
                @"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
      {
          ""type"": ""Container"",
          ""items"": [
            {
              ""type"": ""TextBlock"",
              ""text"": ""Matt""
            }
            ]
        }
    ]
}"; 

            string cardJson = transformer.Transform(testString, null);
            AssertJsonEqual(cardJson, expectedString);
        }
        [TestMethod]
        public void TestInlineMemoryScope2()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var testString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""version"": ""1.0"",
                    ""body"": [
                      {
                          ""type"": ""Container"",
                          ""$data"": { ""name"": ""Matt"" }, 
                          ""items"": [
                              {
                                  ""type"": ""TextBlock"",
                                  ""text"": ""${name}""
                              }
                          ]
                      }
                    ]
                }";
            var expectedString =
                @"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
      {
          ""type"": ""Container"",
          ""items"": [
            {
              ""type"": ""TextBlock"",
              ""text"": ""Matt""
            }
            ]
        }
    ]
}"; 

            string cardJson = transformer.Transform(testString, null);
            AssertJsonEqual(cardJson, expectedString);
        }

        [TestMethod]
        public void TestInlineMemoryScope3()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();
            var testString =
                @"{
                    ""type"": ""AdaptiveCard"",
                    ""version"": ""1.0"",
                    ""body"": [
                      {
                          ""type"": ""Container"",
                          ""$data"": { ""name"": ""Andrew"" }, 
                          ""items"": [
                              {
                                  ""type"": ""TextBlock"",
                                  ""$data"": { ""name"": ""Matt"" }, 
                                  ""text"": ""${name}""
                              }
                          ]
                      }
                    ]
                }";
            var expectedString =
                @"{
    ""type"": ""AdaptiveCard"",
    ""version"": ""1.0"",
    ""body"": [
      {
          ""type"": ""Container"",
          ""items"": [
            {
              ""type"": ""TextBlock"",
              ""text"": ""Matt""
            }
            ]
        }
    ]
}"; 

            string cardJson = transformer.Transform(testString, null);
            AssertJsonEqual(expectedString, cardJson);
        }
    }
    [TestClass]
    public class TestRoot
    {
        [TestMethod]
        public void TestRootInDataContext()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
            ""type"": ""AdaptiveCard"",
            ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                  {
                    ""$data"": ""${$root.LineItems}"",
                    ""type"": ""TextBlock"",
                    ""id"": ""ReceiptRequired${$index}"",
                    ""text"": ""${Milage}""
                  }
                ]
              }
            ]
        }";
            string jsonData = @"{
              ""LineItems"": [
                    {""Milage"" : 1},
                    {""Milage"" : 10}
                ]
            }";

            string cardJson = transformer.Transform(jsonTemplate, jsonData);

            TestTemplate.AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                {
                    ""type"": ""TextBlock"",
                    ""id"": ""ReceiptRequired0"",
                    ""text"": ""1""
                },
                {
                    ""type"": ""TextBlock"",
                    ""id"": ""ReceiptRequired1"",
                    ""text"": ""10""
                }
            ]
        }
    ]
}", cardJson);
        }

        [TestMethod]
        public void TestCanAccessByAEL()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
            ""type"": ""AdaptiveCard"",
            ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                  {
                    ""type"": ""TextBlock"",
                    ""text"": ""${$root.LineItems[0].Milage}""
                  },
                  {
                    ""type"": ""TextBlock"",
                    ""text"": ""${$root.LineItems[1].Milage}""
                  }
                ]
              }
            ]
        }";
            string jsonData = @"{
              ""LineItems"": [
                    {""Milage"" : 1},
                    {""Milage"" : 10}
                ]
            }";

            string cardJson = transformer.Transform(jsonTemplate, jsonData);

            TestTemplate.AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                {
                    ""type"": ""TextBlock"",
                    ""text"": ""1""
                },
                {
                    ""type"": ""TextBlock"",
                    ""text"": ""10""
                }
            ]
        }
    ]
}", cardJson);
        }

        [TestMethod]
        public void TestWorkWithRepeatingItems()
        {
            AdaptiveTransformer transformer = new AdaptiveTransformer();

            string jsonTemplate = @"{
            ""type"": ""AdaptiveCard"",
            ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                  {
                    ""$data"": ""${$root.LineItems}"",
                    ""type"": ""TextBlock"",
                    ""text"": ""Class: ${$root.Class}, Mileage: ${Mileage}""
                  }
                ]
              }
            ]
        }";
            string jsonData = @"{
              ""Class"" : ""Ship"",
              ""LineItems"": [
                    {""Mileage"" : 1},
                    {""Mileage"" : 10}
                ]
            }";

            string cardJson = transformer.Transform(jsonTemplate, jsonData);

            TestTemplate.AssertJsonEqual(@"{
    ""type"": ""AdaptiveCard"",
    ""body"": [
              {
                ""type"": ""Container"",
                ""items"": [
                {
                    ""type"": ""TextBlock"",
                    ""text"": ""10""
                }
            ]
        }
    ]
}", cardJson);
        }
    }

    [TestClass]
    public class TestPartialResult
    {
        [TestMethod]
        public void TestCreation()
        {
            JSONTemplateVisitorResult result = new JSONTemplateVisitorResult();
            result.Append("hello world");
            Assert.AreEqual(result.ToString(), "hello world");
        }

        [TestMethod]
        public void TestMerging()
        {
            JSONTemplateVisitorResult result1 = new JSONTemplateVisitorResult();
            result1.Append("hello");

            JSONTemplateVisitorResult result2 = new JSONTemplateVisitorResult();
            result2.Append(" world");


            JSONTemplateVisitorResult result3 = new JSONTemplateVisitorResult();
            result3.Append("!");

            result1.Append(result2);
            result1.Append(result3);

            Assert.AreEqual(result1.ToString(), "hello world!");
        }

        [TestMethod]
        public void TestCreationOfPartialResult()
        {
            JSONTemplateVisitorResult result1 = new JSONTemplateVisitorResult();
            result1.Append("hello");

            JSONTemplateVisitorResult result2 = new JSONTemplateVisitorResult();
            result2.Append("name", false);

            JSONTemplateVisitorResult result3 = new JSONTemplateVisitorResult();
            result3.Append("!");

            result1.Append(result2);
            result1.Append(result3);

            Assert.AreEqual(result1.ToString(), "hello{name}!");
        }
        public void TestCreationOfWhen()
        {
            JSONTemplateVisitorResult result1 = new JSONTemplateVisitorResult();
            result1.Append("hello");

            JSONTemplateVisitorResult result2 = new JSONTemplateVisitorResult();
            result2.Append("name", false);

            JSONTemplateVisitorResult result3 = new JSONTemplateVisitorResult();
            result3.Append("!");

            result1.Append(result2);
            result1.Append(result3);

            Assert.AreEqual(result1.ToString(), "hello{name}!");
        }
    }

    [TestClass]
    public class TestCEL
    {
        [TestMethod]
        public void TestCreation()
        {
            string jsonData = @"{
            ""person"": {
                ""firstName"": ""Super"",
                ""lastName"": ""man""
                }
            }";

            JToken token = JToken.Parse(jsonData);
            var (value, error) = new ValueExpression("${person.firstName}man").TryGetValue(token as JObject);
            Assert.AreEqual("Superman", value);
        }

        [TestMethod]
        public void TestIndex()
        {
            string jsonData = @"{
            ""$index"": 0
            }";

            JToken token = JToken.Parse(jsonData);
            var (value, error) = new ValueExpression("${$index}}").TryGetValue(token as JObject);
            Assert.AreEqual("0", value);
        }

        [TestMethod]
        public void TestSimpleToString()
        {
            string jsonData = @"{
            ""person"": {
                ""firstName"": ""Super"",
                ""lastName"": ""man"",
                ""age"" : 79 
                }
            }";

            Stopwatch.StartNew();
            var beginTime0 = Stopwatch.GetTimestamp();
            JToken token = JToken.Parse(jsonData);
            var endTime0 = Stopwatch.GetTimestamp();
            Console.WriteLine("time0 took: " + (endTime0 - beginTime0));
            var beginTime1 = Stopwatch.GetTimestamp();
            var (value, error) = new ValueExpression("${string(person.age)}").TryGetValue(token as JObject);
            var endTime1 = Stopwatch.GetTimestamp();
            Console.WriteLine("time1 took: " + (endTime1 - beginTime1));
            Assert.AreEqual("79", value);
        }
    }
}
