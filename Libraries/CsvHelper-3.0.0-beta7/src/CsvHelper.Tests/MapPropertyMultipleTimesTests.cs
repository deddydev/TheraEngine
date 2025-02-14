﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvHelper.Tests
{
	[TestClass]
	public class MapPropertyMultipleTimesTests
	{
		[TestMethod]
		public void MapProperitesToMultipleFieldsWhenWritingTest()
		{
			using( var stream = new MemoryStream() )
			using( var reader = new StreamReader( stream ) )
			using( var writer = new StreamWriter( stream ) )
			using( var csv = new CsvWriter( writer ) )
			{
				var list = new List<Test>
				{
					new Test { Id = 1, Name = "one" }
				};

				csv.Configuration.RegisterClassMap<TestMap>();
				csv.WriteRecords( list );
				writer.Flush();
				stream.Position = 0;

				var expected = new StringBuilder();
				expected.AppendLine( "Id1,Name1,Id2,Name2" );
				expected.AppendLine( "1,one,1,one" );

				var result = reader.ReadToEnd();

				Assert.AreEqual( expected.ToString(), result );
			}
		}

		[TestMethod]
		public void MapPropertiesToMultipleFieldsWhenReadingTest()
		{
			// This is not something that anyone should do, but this
			// is the expected behavior if they do.

			using( var stream = new MemoryStream() )
			using( var reader = new StreamReader( stream ) )
			using( var writer = new StreamWriter( stream ) )
			using( var csv = new CsvReader( reader ) )
			{
				writer.WriteLine( "Id1,Name1,Id2,Name2" );
				writer.WriteLine( "1,one,2,two" );
				writer.Flush();
				stream.Position = 0;

				csv.Configuration.RegisterClassMap<TestMap>();
				var records = csv.GetRecords<Test>().ToList();

				Assert.AreEqual( 2, records[0].Id );
				Assert.AreEqual( "two", records[0].Name );
			}
		}

		private class Test
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		private sealed class TestMap : CsvClassMap<Test>
		{
			public TestMap()
			{
				Map( m => m.Id ).Name( "Id1" );
				Map( m => m.Name ).Name( "Name1" );
				Map( m => m.Id, false ).Name( "Id2" );
				Map( m => m.Name, false ).Name( "Name2" );
			}
		}
	}
}
