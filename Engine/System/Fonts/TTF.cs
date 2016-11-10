using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Fonts
{
    public class TTFHeader
    {
        public buint _scalerType;      //how to extract glyph data
        public bushort _numTables;     //number of tables
        public bushort _searchRange;   //(maximum power of 2 <= numTables)*16
        public bushort _entrySelector; //log2(maximum power of 2 <= numTables)
        public bushort _rangeShift;    //numTables*16-searchRange
    }
    public enum TTFRequiredTable
    {
        cmap,   //character to glyph mapping
        glyf,	//glyph data
        head, 	//font header
        hhea,	//horizontal header
        hmtx,	//horizontal metrics
        loca,	//index to location
        maxp,	//maximum profile
        name,	//naming
        post,	//PostScript
    }
    public enum TTFOptionalTable
    {
        cvt, 	//control value
        fpgm, 	//font program
        hdmx, 	//horizontal device metrics
        vdmx, 	//vertical device metrics
        kern, 	//kerning
        OS_2, 	//OS/2
        prep, 	//control value program
    }
    public unsafe class TTFTable
    {
        public buint _tag;      //identifier of what the data holds
        public buint _checksum; //checksum of table data
        public buint _offset;   //offset to table data, from the beginning of sfnt
        public buint _length;   //length of unpadded table data

        public void CalcTableChecksum(buint* table, uint numberOfBytesInTable)
        {
            uint sum = 0;
            uint nLongs = (numberOfBytesInTable + 3) / 4;
            while (nLongs-- > 0)
                sum += *table++;
            _checksum = sum;
        }
    }
    public unsafe class TTFHeadTable
    {
        public buint version;               //0x00010000 if (version 1.0)
        public buint fontRevision;          //set by font manufacturer
        public buint checkSumAdjustment;    //To compute: set it to 0, calculate the checksum for the 'head' table and put it in the table directory, sum the entire font as a uint32_t, then store 0xB1B0AFBA - sum. (The checksum for the 'head' table will be wrong as a result.That is OK; do not reset it.)
        public buint magicNumber;           //set to 0x5F0F3CF5
        public bushort flags;
        //bit 0 - y value of 0 specifies baseline
        //bit 1 - x position of left most black bit is LSB
        //bit 2 - scaled point size and actual point size will differ(i.e. 24 point glyph differs from 12 point glyph scaled by factor of 2)
        //bit 3 - use integer scaling instead of fractional
        //bit 4 - (used by the Microsoft implementation of the TrueType scaler)
        //bit 5 - This bit should be set in fonts that are intended to e laid out vertically, and in which the glyphs have been drawn such that an x-coordinate of 0 corresponds to the desired vertical baseline.
        //bit 6 - This bit must be set to zero.
        //bit 7 - This bit should be set if the font requires layout for correct linguistic rendering (e.g.Arabic fonts).
        //bit 8 - This bit should be set for an AAT font which has one or more metamorphosis effects designated as happening by default.
        //bit 9 - This bit should be set if the font contains any strong right-to-left glyphs.
        //bit 10 - This bit should be set if the font contains Indic-style rearrangement effects.
        //bits 11-13 - Defined by Adobe.
        //bit 14 - This bit should be set if the glyphs in the font are simply generic symbols for code point ranges, such as for a last resort font.
        public bushort unitsPerEm;      //range from 64 to 16384
        public bulong created;          //international date
        public bulong modified;         //international date
        public bshort xMin;             //for all glyph bounding boxes
        public bshort yMin;             //for all glyph bounding boxes
        public bshort xMax;             //for all glyph bounding boxes
        public bshort yMax;             //for all glyph bounding boxes
        public bushort macStyle;
        //bit 0 bold
        //bit 1 italic
        //bit 2 underline
        //bit 3 outline
        //bit 4 shadow
        //bit 5 condensed(narrow)
        //bit 6 extended
        public bushort lowestRecPPEM;       //smallest readable size in pixels
        public bshort fontDirectionHint;
        //0 Mixed directional glyphs
        //1 Only strongly left to right glyphs
        //2 Like 1 but also contains neutrals
        //-1 Only strongly right to left glyphs
        //-2 Like -1 but also contains neutrals
        public bshort indexToLocFormat;     //0 for short offsets, 1 for long
        public bshort glyphDataFormat; 	    //0 for current format
    }
}
