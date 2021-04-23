using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO.Json;
using System;

namespace Neo.UnitTests.IO.Json
{
    [TestClass]
    public class UT_JPath
    {
        [TestMethod]
        public void TestRecursiveDescent()
        {
            JObject json = new()
            {
                ["a"] = 1,
                ["b"] = new JObject()
                {
                    ["a"] = 2
                }
            };
            JArray array = json.JsonPath("$..a");
            Assert.AreEqual("[1,2]", array.ToString());
        }

        [TestMethod]
        public void TestJsonPath()
        {
            var json = JObject.Parse(@"
{
    ""store"": {
        ""book"": [
            {
                ""category"": ""reference"",
                ""author"": ""Nigel Rees"",
                ""title"": ""Sayings of the Century"",
                ""price"": 8.95
            },
            {
                ""category"": ""fiction"",
                ""author"": ""Evelyn Waugh"",
                ""title"": ""Sword of Honour"",
                ""price"": 12.99
            },
            {
                ""category"": ""fiction"",
                ""author"": ""Herman Melville"",
                ""title"": ""Moby Dick"",
                ""isbn"": ""0-553-21311-3"",
                ""price"": 8.99
            },
            {
                ""category"": ""fiction"",
                ""author"": ""J. R. R. Tolkien"",
                ""title"": ""The Lord of the Rings"",
                ""isbn"": ""0-395-19395-8"",
                ""price"": 22.99
            }
        ],
        ""bicycle"": {
                ""color"": ""red"",
            ""price"": 19.95
        }
        },
    ""expensive"": 10
}");

            // Test

            Assert.AreEqual(@"[""Nigel Rees"",""Evelyn Waugh"",""Herman Melville"",""J. R. R. Tolkien""]", json.JsonPath("$.store.book[*].author").ToString());
            Assert.AreEqual(@"[[{""category"":""reference"",""author"":""Nigel Rees"",""title"":""Sayings of the Century"",""price"":8.95},{""category"":""fiction"",""author"":""Evelyn Waugh"",""title"":""Sword of Honour"",""price"":12.99},{""category"":""fiction"",""author"":""Herman Melville"",""title"":""Moby Dick"",""isbn"":""0-553-21311-3"",""price"":8.99},{""category"":""fiction"",""author"":""J. R. R. Tolkien"",""title"":""The Lord of the Rings"",""isbn"":""0-395-19395-8"",""price"":22.99}],{""color"":""red"",""price"":19.95}]", json.JsonPath("$.store.*").ToString());

            Assert.AreEqual(@"[]", json.JsonPath("$..author").ToString()); // Wrong (All authors)
            Assert.AreEqual(@"[19.95]", json.JsonPath("$.store..price").ToString()); // Wrong (The price of everything)

            // TODO

            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[2]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[-2]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[0,1]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[:2]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[1:2]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[-2:]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[2:]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[?(@.isbn)]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$.store.book[?(@.price < 10)]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[?(@.price <= $['expensive'])]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book[?(@.author =~ /.*REES/i)]").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..*").ToString());
            //Assert.AreEqual(@"[1,2]", json.JsonPath("$..book.length()").ToString());
        }

        [TestMethod]
        public void TestJSONPathIndex()
        {
            var obj = JObject.Parse("[\"a\",\"b\",\"c\",\"d\"]");
            
            Assert.AreEqual("[\"a\"]", obj.JsonPath("$[0]").ToString());
            Assert.AreEqual("[\"d\"]", obj.JsonPath("$[3]").ToString());
            Assert.AreEqual("[\"b\"]", obj.JsonPath("$[1:2]").ToString());
            Assert.AreEqual("[\"b\",\"c\"]", obj.JsonPath("$[1:-1]").ToString());
            Assert.AreEqual("[\"b\",\"c\"]", obj.JsonPath("$[-3:-1]").ToString());
            Assert.AreEqual("[\"b\",\"c\"]", obj.JsonPath("$[-3:3]").ToString());
            Assert.AreEqual("[\"a\",\"b\",\"c\"]", obj.JsonPath("$[:3]").ToString());
            Assert.AreEqual("[\"a\",\"b\",\"c\",\"d\"]", obj.JsonPath("$[:100]").ToString());
            Assert.AreEqual("[\"b\",\"c\",\"d\"]", obj.JsonPath("$[1:]").ToString());

            obj = JObject.Parse("[[1,2],{\"1\":\"4\"},[5,6]]");
            Assert.AreEqual("[2,6]", obj.JsonPath("$[:][1]").ToString());
            Assert.AreEqual("[1,2,\"4\",5,6]", obj.JsonPath("$[*].*").ToString());

            obj = JObject.Parse("[[1,2],3,{\"1\":\"4\"},[5,6]]");
            Assert.AreEqual("[2,6]", obj.JsonPath("$[*][1:]").ToString());
        }

        [TestMethod]
        public void TestJSONPathIdent()
        {
            var obj = JObject.Parse(@"{
		        ""store"": {
                    ""name"": ""big"",
                    ""sub"": [
                        { ""name"": ""sub1"" },
			            { ""name"": ""sub2"" }
                    ],
                    ""partner"": { ""name"": ""ppp"" }
		        },
                ""another"": { ""name"": ""small"" }
	        }");
            
            Assert.AreEqual("[\"big\"]", obj.JsonPath("$.store.name").ToString());
            Assert.AreEqual("[\"big\"]", obj.JsonPath("$['store']['name']").ToString());

            // map key order
            //Assert.AreEqual("[\"small\",\"big\"]", obj.JsonPath("$[*].name").ToString());
            //Assert.AreEqual("[\"small\",\"big\"]", obj.JsonPath("$.*.name").ToString());

            Assert.AreEqual("[\"big\"]", obj.JsonPath("$..store.name").ToString());
            //Assert.AreEqual("[\"big\",\"ppp\",\"sub1\",\"sub2\"]", obj.JsonPath("$.store..name").ToString());
            Assert.AreEqual("[\"sub1\",\"sub2\"]", obj.JsonPath("$..sub[*].name").ToString());
            Assert.AreEqual("[\"ppp\",\"sub1\",\"sub2\"]", obj.JsonPath("$.store..*.name").ToString());
            Assert.AreEqual("[]", obj.JsonPath("$..sub.name").ToString());
            //Assert.AreEqual("[\"sub1\",\"sub2\"]", obj.JsonPath("$..sub..name").ToString());
        }

         [TestMethod]
        public void TestJSONPathUnion()
        {
            var obj = JObject.Parse(@"[""a"",{""x"":1,""y"":2,""z"":3},""c"",""d""]");
            
            Assert.AreEqual(@"[""a"",""c""]", obj.JsonPath("$[0,2]").ToString());
            Assert.AreEqual("[1,3]", obj.JsonPath("$[1]['x','z']").ToString());
        }

        [TestMethod]
        public void TestJSONPathInvalid()
        {
            var obj = new JObject();
            string bigNum = int.MaxValue.ToString() + "1";

            Assert.ThrowsException<FormatException>(() => obj.JsonPath("."));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$1"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("&"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$&"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$.&"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$.[0]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$..&"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$..1"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[&]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[**]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1&]"));
            Assert.ThrowsException<OverflowException>(() => obj.JsonPath("$[" + bigNum + "]"));
            Assert.ThrowsException<OverflowException>(() => obj.JsonPath("$[" + bigNum + ":]"));
            Assert.ThrowsException<OverflowException>(() => obj.JsonPath("$[:" + bigNum + "]"));
            Assert.ThrowsException<OverflowException>(() => obj.JsonPath("$[1," + bigNum + "]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[" + bigNum + "[]]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$['a'&]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$['a'1]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$['a"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$['\\u123']"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$['s','\\u123']"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[[]]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1,'a']"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1,1&"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1,1[]]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1:&]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1:1[]]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1:[]]"));
            Assert.ThrowsException<FormatException>(() => obj.JsonPath("$[1:[]]"));
        }
    }
}
