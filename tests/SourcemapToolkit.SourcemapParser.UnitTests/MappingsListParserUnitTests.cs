﻿
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class MappingsListParserUnitTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseSingleMappingSegment_NullSegmentFields_ThrowArgumentNullException()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = null;

			// Act
			mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseSingleMappingSegment_0SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int>();

			// Act
			mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseSingleMappingSegment_2SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 2 };

			// Act
			mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseSingleMappingSegment_3SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 2, 3 };

			// Act
			mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);
		}

		[TestMethod]
		public void ParseSingleMappingSegment_NoPreviousStateSingleSegment_GeneratedColumnSet()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 16 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(16, result.GeneratedColumnNumber);
			Assert.IsFalse(result.OriginalSourceFileIndex.HasValue);
			Assert.IsFalse(result.OriginalLineNumber.HasValue);
			Assert.IsFalse(result.OriginalColumnNumber.HasValue);
			Assert.IsFalse(result.OriginalNameIndex.HasValue);
		}

		[TestMethod]
		public void ParseSingleMappingSegment_NoPreviousState4Segments_OriginalNameIndexNotSetInMappingEntry()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 1, 2, 4 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(1, result.GeneratedColumnNumber);
			Assert.AreEqual(1, result.OriginalSourceFileIndex);
			Assert.AreEqual(2, result.OriginalLineNumber);
			Assert.AreEqual(4, result.OriginalColumnNumber);
			Assert.IsFalse(result.OriginalNameIndex.HasValue);
		}

		[TestMethod]
		public void ParseSingleMappingSegment_NoPreviousState5Segments_AllFieldsSetInMappingEntry()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 3, 6, 10, 15 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(1, result.GeneratedColumnNumber);
			Assert.AreEqual(3, result.OriginalSourceFileIndex);
			Assert.AreEqual(6, result.OriginalLineNumber);
			Assert.AreEqual(10, result.OriginalColumnNumber);
			Assert.AreEqual(15, result.OriginalNameIndex);
		}

		[TestMethod]
		public void ParseSingleMappingSegment_HasPreviousState5Segments_AllFieldsSetIncrementally()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState(newGeneratedColumnBase: 6,
				newSourcesListIndexBase: 7,
				newOriginalSourceStartingLineBase: 8,
				newOriginalSourceStartingColumnBase: 9,
				newNamesListIndexBase: 10);
				List <int> segmentFields = new List<int> { 1, 2, 3, 4, 5 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(7, result.GeneratedColumnNumber);
			Assert.AreEqual(9, result.OriginalSourceFileIndex);
			Assert.AreEqual(11, result.OriginalLineNumber);
			Assert.AreEqual(13, result.OriginalColumnNumber);
			Assert.AreEqual(15, result.OriginalNameIndex);
		}

		[TestMethod]
		public void ParseMappings_SingleSemicolon_GeneratedLineNumberNotIncremented()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			string mappingsString = "AAaAA,CAACC;";

			// Act
			List<MappingEntry> mappingsList = mappingsListParser.ParseMappings(mappingsString);

			// Assert
			Assert.AreEqual(2, mappingsList.Count);

			Assert.AreEqual(0, mappingsList[0].GeneratedLineNumber);
			Assert.AreEqual(0, mappingsList[0].GeneratedColumnNumber);
			Assert.AreEqual(0, mappingsList[0].OriginalSourceFileIndex);
			Assert.AreEqual(13, mappingsList[0].OriginalLineNumber);
			Assert.AreEqual(0, mappingsList[0].OriginalColumnNumber);
			Assert.AreEqual(0, mappingsList[0].OriginalNameIndex);

			Assert.AreEqual(0, mappingsList[1].GeneratedLineNumber);
			Assert.AreEqual(1, mappingsList[1].GeneratedColumnNumber);
			Assert.AreEqual(0, mappingsList[1].OriginalSourceFileIndex);
			Assert.AreEqual(13, mappingsList[1].OriginalLineNumber);
			Assert.AreEqual(1, mappingsList[1].OriginalColumnNumber);
			Assert.AreEqual(1, mappingsList[1].OriginalNameIndex);
		}

		[TestMethod]
		public void ParseMappings_TwoSemicolons_GeneratedLineNumberIncremented()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			string mappingsString = "CEGegB;CACf;";

			// Act
			List<MappingEntry> mappingsList = mappingsListParser.ParseMappings(mappingsString);

			// Assert
			Assert.AreEqual(2, mappingsList.Count);

			Assert.AreEqual(0, mappingsList[0].GeneratedLineNumber);
			Assert.AreEqual(1, mappingsList[0].GeneratedColumnNumber);
			Assert.AreEqual(2, mappingsList[0].OriginalSourceFileIndex);
			Assert.AreEqual(3, mappingsList[0].OriginalLineNumber);
			Assert.AreEqual(15, mappingsList[0].OriginalColumnNumber);
			Assert.AreEqual(16, mappingsList[0].OriginalNameIndex);

			Assert.AreEqual(1, mappingsList[1].GeneratedLineNumber);
			Assert.AreEqual(1, mappingsList[1].GeneratedColumnNumber);
			Assert.AreEqual(2, mappingsList[1].OriginalSourceFileIndex);
			Assert.AreEqual(4, mappingsList[1].OriginalLineNumber);
			Assert.AreEqual(0, mappingsList[1].OriginalColumnNumber);
			Assert.IsFalse(mappingsList[1].OriginalNameIndex.HasValue);
		}
	}
}