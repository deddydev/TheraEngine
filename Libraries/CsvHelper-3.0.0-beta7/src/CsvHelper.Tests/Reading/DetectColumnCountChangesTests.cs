﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsvHelper.Tests.Reading
{
	[TestClass]
	public class DetectColumnCountChangesTests
	{
		[TestMethod]
		public void ConsistentColumnsWithDetectColumnChangesTest()
		{
			using( var stream = new MemoryStream() )
			using( var writer = new StreamWriter( stream ) )
			using( var reader = new StreamReader( stream ) )
			using( var csv = new CsvReader( reader ) )
			{
				writer.WriteLine( "Column 1,Column 2" );
				writer.WriteLine( "1,2" );
				writer.Flush();
				stream.Position = 0;

				csv.Configuration.DetectColumnCountChanges = true;
				while( !csv.Read() )
				{
				}
			}
		}

		[TestMethod]
		public void InconsistentColumnsMultipleRowsTest()
		{
			using( var stream = new MemoryStream() )
			using( var writer = new StreamWriter( stream ) )
			using( var reader = new StreamReader( stream ) )
			using( var csv = new CsvReader( reader ) )
			{
				writer.WriteLine( "Column 1,Column 2" );
				writer.WriteLine( "1,2" ); // Valid
				writer.WriteLine( "1,2,3" ); // Error - too many fields
				writer.WriteLine( "1,2" ); // Valid
				writer.WriteLine( "1" ); // Error - not enough fields
				writer.WriteLine( "1,2,3,4" ); // Error - too many fields
				writer.WriteLine( "1,2" ); // Valid
				writer.WriteLine( "1,2" ); // Valid
				writer.Flush();
				stream.Position = 0;

				csv.Configuration.DetectColumnCountChanges = true;
				var failCount = 0;

				while( true )
				{
					try
					{
						if( !csv.Read() )
						{
							break;
						}
					}
					catch( CsvBadDataException )
					{
						failCount++;
					}
				}

				// Expect only 3 errors
				Assert.AreEqual<int>( 3, failCount );
			}
		}

		[TestMethod]
		public void InconsistentColumnsSmallerTest()
		{
			using( var stream = new MemoryStream() )
			using( var writer = new StreamWriter( stream ) )
			using( var reader = new StreamReader( stream ) )
			using( var csv = new CsvReader( reader ) )
			{
				writer.WriteLine( "1,2,3,4" );
				writer.WriteLine( "5,6,7" );
				writer.Flush();
				stream.Position = 0;

				csv.Configuration.DetectColumnCountChanges = true;
				csv.Read();

				try
				{
					csv.Read();
					Assert.Fail();
				}
				catch( CsvBadDataException )
				{
				}
			}
		}

		[TestMethod]
		public void InconsistentColumnsTest()
		{
			using( var stream = new MemoryStream() )
			using( var writer = new StreamWriter( stream ) )
			using( var reader = new StreamReader( stream ) )
			using( var csv = new CsvReader( reader ) )
			{
				writer.WriteLine( "Column 1,Column 2" );
				writer.WriteLine( "1,2,3" );
				writer.Flush();
				stream.Position = 0;

				csv.Configuration.DetectColumnCountChanges = true;
				csv.Read();

				try
				{
					csv.Read();
					Assert.Fail();
				}
				catch( CsvBadDataException )
				{
				}
			}
		}

		[TestMethod]
		public void WillThrowOnMissingFieldStillWorksTest()
		{
			using( var stream = new MemoryStream() )
			using( var writer = new StreamWriter( stream ) )
			using( var reader = new StreamReader( stream ) )
			using( var csv = new CsvReader( reader ) )
			{
				writer.WriteLine( "1,2,3" );
				writer.WriteLine( "4,5" );
				writer.Flush();
				stream.Position = 0;

				var missingFieldExceptionCount = 0;
				var columnCountChangeExceptionCount = 0;
				csv.Configuration.DetectColumnCountChanges = true;
				csv.Configuration.IgnoreReadingExceptions = true;
				csv.Configuration.RegisterClassMap<TestMap>();
				csv.Configuration.ReadingExceptionCallback = ( ex, r ) =>
				{
					if( ex is CsvMissingFieldException )
					{
						missingFieldExceptionCount++;
					}
					else if( ex is CsvBadDataException )
					{
						columnCountChangeExceptionCount++;
					}
				};
				var records = csv.GetRecords<Test>().ToList();
				Assert.AreEqual( 1, missingFieldExceptionCount );
				Assert.AreEqual( 1, columnCountChangeExceptionCount );
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
				Map( m => m.Id );
				Map( m => m.Name );
			}
		}
	}
}
