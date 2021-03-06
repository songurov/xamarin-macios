//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//   Aaron Bockover
//
// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
#define DOUBLE_BLOCKS
using XamCore.ObjCRuntime;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.CoreGraphics;
#if !WATCH
using XamCore.CoreMedia;
using XamCore.SceneKit;
#endif
using XamCore.Security;
#if IOS
using XamCore.CoreSpotlight;
#endif

#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.CoreLocation;
using XamCore.UIKit;
#endif

using System;
using System.ComponentModel;

#if MONOMAC
// In Apple headers, this is a typedef to a pointer to a private struct
using NSAppleEventManagerSuspensionID = System.IntPtr;
// These two are both four char codes i.e. defined on a uint with constant like 'xxxx'
using AEKeyword = System.UInt32;
using OSType = System.UInt32;
// typedef double NSTimeInterval;
using NSTimeInterval = System.Double;
#endif

// This little gem comes from a btouch bug that wrote the NSFilePresenterReacquirer delegate to the wrong
// namespace for a while (it should go into Foundation, but due to the bug it went into UIKit). In order
// to not break backwards compatibility (once the btouch bug was fixed), we need to make sure the delegate
// stays in UIKit for Xamarin.iOS/Classic (the delegate was always in the right namespace for Xamarin.Mac).
#if XAMCORE_2_0 || MONOMAC
namespace XamCore.Foundation {
#else
namespace XamCore.UIKit {
#endif
	public delegate void NSFilePresenterReacquirer ([BlockCallback] Action reacquirer);
}

namespace XamCore.Foundation
{
#if XAMCORE_2_0
	public delegate NSComparisonResult NSComparator (NSObject obj1, NSObject obj2);
#else
	public delegate int /* !XAMCORE_2_0 */ NSComparator (NSObject obj1, NSObject obj2);
#endif
	public delegate void NSAttributedRangeCallback (NSDictionary attrs, NSRange range, ref bool stop);
	public delegate void NSAttributedStringCallback (NSObject value, NSRange range, ref bool stop);

	public delegate bool NSEnumerateErrorHandler (NSUrl url, NSError error);
	public delegate void NSMetadataQueryEnumerationCallback (NSObject result, nuint idx, ref bool stop);
	public delegate void NSItemProviderCompletionHandler (NSObject itemBeingLoaded, NSError error);
	public delegate void NSItemProviderLoadHandler ([BlockCallback] NSItemProviderCompletionHandler completionHandler, Class expectedValueClass, NSDictionary options);
	public delegate void EnumerateDatesCallback (NSDate date, bool exactMatch, ref bool stop);
	public delegate void EnumerateIndexSetCallback (nuint idx, ref bool stop);

	public interface NSArray<TValue> : NSArray {}

	[BaseType (typeof (NSObject))]
	public interface NSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration {
		[Export ("count")]
		nuint Count { get; }

		[Export ("objectAtIndex:")]
		IntPtr ValueAt (nuint idx);

		[Static]
		[Internal]
		[Export ("arrayWithObjects:count:")]
		IntPtr FromObjects (IntPtr array, nint count);

		[Export ("valueForKey:")]
		[MarshalNativeExceptions]
		NSObject ValueForKey (NSString key);

		[Export ("setValue:forKey:")]
		void SetValueForKey (NSObject value, NSString key);

		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Export ("arrayWithContentsOfFile:")][Static]
		NSArray FromFile (string path);
		
		[Export ("sortedArrayUsingComparator:")]
		NSArray Sort (NSComparator cmptr);
		
		[Export ("filteredArrayUsingPredicate:")]
		NSArray Filter (NSPredicate predicate);

		[Internal]
		[Sealed]
		[Export ("containsObject:")]
		bool _Contains (IntPtr anObject);

		[Export ("containsObject:")]
		bool Contains (NSObject anObject);

		[Internal]
		[Sealed]
		[Export ("indexOfObject:")]
		nuint _IndexOf (IntPtr anObject);

		[Export ("indexOfObject:")]
		nuint IndexOf (NSObject anObject);

		[Export ("addObserver:toObjectsAtIndexes:forKeyPath:options:context:")]
		void AddObserver (NSObject observer, NSIndexSet indexes, string keyPath, NSKeyValueObservingOptions options, IntPtr context);

		[Export ("removeObserver:fromObjectsAtIndexes:forKeyPath:context:")]
		void RemoveObserver (NSObject observer, NSIndexSet indexes, string keyPath, IntPtr context);

		[Export ("removeObserver:fromObjectsAtIndexes:forKeyPath:")]
		void RemoveObserver (NSObject observer, NSIndexSet indexes, string keyPath);
	}

#if MONOMAC
	public interface NSAttributedStringDocumentAttributes { }
#endif

	[Since (3,2)]
	[BaseType (typeof (NSObject))]
	public partial interface NSAttributedString : NSCoding, NSMutableCopying, NSSecureCoding
	#if MONOMAC
		, NSPasteboardReading, NSPasteboardWriting
	#endif
	{
#if !WATCH
		[Static, Export ("attributedStringWithAttachment:")]
		NSAttributedString FromAttachment (NSTextAttachment attachment);
#endif

		[Export ("string")]
		IntPtr LowLevelValue { get; }

		[Export ("attributesAtIndex:effectiveRange:")]
		IntPtr LowLevelGetAttributes (nint location, out NSRange effectiveRange);

		[Export ("length")]
		nint Length { get; }

		// TODO: figure out the type, this deserves to be strongly typed if possble
		[Export ("attribute:atIndex:effectiveRange:")]
		NSObject GetAttribute (string attribute, nint location, out NSRange effectiveRange);

		[Export ("attributedSubstringFromRange:"), Internal]
		NSAttributedString Substring (NSRange range);

		[Export ("attributesAtIndex:longestEffectiveRange:inRange:")]
		NSDictionary GetAttributes (nint location, out NSRange longestEffectiveRange, NSRange rangeLimit);

		[Export ("attribute:atIndex:longestEffectiveRange:inRange:")]
		NSObject GetAttribute (string attribute, nint location, out NSRange longestEffectiveRange, NSRange rangeLimit);

		[Export ("isEqualToAttributedString:")]
		bool IsEqual (NSAttributedString other);

		[Export ("initWithString:")]
		IntPtr Constructor (string str);

#if !MONOMAC

#if IOS
		// New API in iOS9 with same signature as an older alternative.
		// We expose only the *new* one for the new platforms as the old
		// one was moved to `NSDeprecatedKitAdditions (NSAttributedString)`
		[iOS (9,0)]
		[Internal]
		[Export ("initWithURL:options:documentAttributes:error:")]
		IntPtr InitWithURL (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
		// but we still need to allow the API to work before iOS 9.0
		// and to compleify matters the old one was deprecated in 9.0
		[iOS (7,0)]
		[Internal]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Export ("initWithFileURL:options:documentAttributes:error:")]
		IntPtr InitWithFileURL (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#elif TVOS || WATCH
		[iOS (9,0)]
		[Export ("initWithURL:options:documentAttributes:error:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif

		[Since (7,0)]
		[Wrap ("this (url, options == null ? null : options.Dictionary, out resultDocumentAttributes, ref error)")]
		IntPtr Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);

		[Since (7,0)]
		[Export ("initWithData:options:documentAttributes:error:")]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);

		[Since (7,0)]
		[Wrap ("this (data, options == null ? null : options.Dictionary, out resultDocumentAttributes, ref error)")]
		IntPtr Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);

		[Since (7,0)]
		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetDataFromRange (NSRange range, NSDictionary attributes, ref NSError error);

		[Since (7,0)]
		[Wrap ("GetDataFromRange (range, documentAttributes == null ? null : documentAttributes.Dictionary, ref error)")]
		NSData GetDataFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);

		[Since (7,0)]
		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSDictionary attributes, ref NSError error);

		[Since (7,0)]
		[Wrap ("GetFileWrapperFromRange (range, documentAttributes == null ? null : documentAttributes.Dictionary, ref error)")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);

#endif
		
		[Export ("initWithString:attributes:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		IntPtr Constructor (string str, [NullAllowed] NSDictionary attributes);

		[Export ("initWithAttributedString:")]
		IntPtr Constructor (NSAttributedString other);

		[Export ("enumerateAttributesInRange:options:usingBlock:")]
		void EnumerateAttributes (NSRange range, NSAttributedStringEnumeration options, NSAttributedRangeCallback callback);

		[Export ("enumerateAttribute:inRange:options:usingBlock:")]
		void EnumerateAttribute (NSString attributeName, NSRange inRange, NSAttributedStringEnumeration options, NSAttributedStringCallback callback);


#if MONOMAC && !XAMCORE_2_0
		[Field ("NSFontAttributeName", "AppKit")]
		NSString FontAttributeName { get; }

		[Field ("NSLinkAttributeName", "AppKit")]
		NSString LinkAttributeName { get; }

		[Field ("NSUnderlineStyleAttributeName", "AppKit")]
		NSString UnderlineStyleAttributeName { get; }

		[Field ("NSStrikethroughStyleAttributeName", "AppKit")]
		NSString StrikethroughStyleAttributeName { get; }

		[Field ("NSStrokeWidthAttributeName", "AppKit")]
		NSString StrokeWidthAttributeName { get; }

		[Field ("NSParagraphStyleAttributeName", "AppKit")]
		NSString ParagraphStyleAttributeName { get; }

		[Field ("NSForegroundColorAttributeName", "AppKit")]
		NSString ForegroundColorAttributeName { get; }

		[Field ("NSBackgroundColorAttributeName", "AppKit")]
		NSString BackgroundColorAttributeName { get; }

		[Field ("NSLigatureAttributeName", "AppKit")]
		NSString LigatureAttributeName { get; } 

		[Field ("NSObliquenessAttributeName", "AppKit")]
		NSString ObliquenessAttributeName { get; }

		[Field ("NSSuperscriptAttributeName", "AppKit")]
		NSString SuperscriptAttributeName { get; }

		[Field ("NSAttachmentAttributeName", "AppKit")]
		NSString AttachmentAttributeName { get; }
		
		[Field ("NSBaselineOffsetAttributeName", "AppKit")]
		NSString BaselineOffsetAttributeName { get; }
		
		[Field ("NSKernAttributeName", "AppKit")]
		NSString KernAttributeName { get; }
		
		[Field ("NSStrokeColorAttributeName", "AppKit")]
		NSString StrokeColorAttributeName { get; }
		
		[Field ("NSUnderlineColorAttributeName", "AppKit")]
		NSString UnderlineColorAttributeName { get; }
		
		[Field ("NSStrikethroughColorAttributeName", "AppKit")]
		NSString StrikethroughColorAttributeName { get; }
		
		[Field ("NSShadowAttributeName", "AppKit")]
		NSString ShadowAttributeName { get; }
		
		[Field ("NSExpansionAttributeName", "AppKit")]
		NSString ExpansionAttributeName { get; }
		
		[Field ("NSCursorAttributeName", "AppKit")]
		NSString CursorAttributeName { get; }
		
		[Field ("NSToolTipAttributeName", "AppKit")]
		NSString ToolTipAttributeName { get; }
		
		[Field ("NSMarkedClauseSegmentAttributeName", "AppKit")]
		NSString MarkedClauseSegmentAttributeName { get; }
		
		[Field ("NSWritingDirectionAttributeName", "AppKit")]
		NSString WritingDirectionAttributeName { get; }
		
		[Field ("NSVerticalGlyphFormAttributeName", "AppKit")]
		NSString VerticalGlyphFormAttributeName { get; }
#endif

#if MONOMAC
		[Export("size")]
		CGSize Size { get; }

		[Export ("initWithData:options:documentAttributes:error:")]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary options, out NSDictionary docAttributes, out NSError error);

		[Export ("initWithDocFormat:documentAttributes:")]
		IntPtr Constructor(NSData wordDocFormat, out NSDictionary docAttributes);

		[Export ("initWithHTML:baseURL:documentAttributes:")]
		IntPtr Constructor (NSData htmlData, NSUrl baseUrl, out NSDictionary docAttributes);
		
		[Export ("drawAtPoint:")]
		void DrawString (CGPoint point);
		
		[Export ("drawInRect:")]
		void DrawString (CGRect rect);
		
		[Export ("drawWithRect:options:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options);	

		[Export ("initWithURL:options:documentAttributes:error:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, out NSError error);

		[Wrap ("this (url, options == null ? null : options.Dictionary, out resultDocumentAttributes, out error)")]
		IntPtr Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);

		[Wrap ("this (data, options == null ? null : options.Dictionary, out resultDocumentAttributes, out error)")]
		IntPtr Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);

		[Export ("initWithPath:documentAttributes:")]
		IntPtr Constructor (string path, out NSDictionary resultDocumentAttributes);

		[Export ("initWithURL:documentAttributes:")]
		IntPtr Constructor (NSUrl url, out NSDictionary resultDocumentAttributes);

		[Internal, Export ("initWithRTF:documentAttributes:")]
		IntPtr InitWithRtf (NSData data, out NSDictionary resultDocumentAttributes);

		[Internal, Export ("initWithRTFD:documentAttributes:")]
		IntPtr InitWithRtfd (NSData data, out NSDictionary resultDocumentAttributes);

		[Internal, Export ("initWithHTML:documentAttributes:")]
		IntPtr InitWithHTML (NSData data, out NSDictionary resultDocumentAttributes);

		[Export ("initWithHTML:options:documentAttributes:")]
		IntPtr Constructor (NSData data, [NullAllowed]  NSDictionary options, out NSDictionary resultDocumentAttributes);

		[Wrap ("this (data, options == null ? null : options.Dictionary, out resultDocumentAttributes)")]
		IntPtr Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes);

		[Export ("initWithRTFDFileWrapper:documentAttributes:")]
		IntPtr Constructor (NSFileWrapper wrapper, out NSDictionary resultDocumentAttributes);

		[Export ("containsAttachments")]
		bool ContainsAttachments { get; }

		[Export ("fontAttributesInRange:")]
		NSDictionary GetFontAttributes (NSRange range);

		[Export ("rulerAttributesInRange:")]
		NSDictionary GetRulerAttributes (NSRange range);

		[Export ("lineBreakBeforeIndex:withinRange:")]
		nuint GetLineBreak (nuint beforeIndex, NSRange aRange);

		[Export ("lineBreakByHyphenatingBeforeIndex:withinRange:")]
		nuint GetLineBreakByHyphenating (nuint beforeIndex, NSRange aRange);

		[Export ("doubleClickAtIndex:")]
		NSRange DoubleClick (nuint index);

		[Export ("nextWordFromIndex:forward:")]
		nuint GetNextWord (nuint fromIndex, bool isForward);

		[Export ("URLAtIndex:effectiveRange:")]
		NSUrl GetUrl (nuint index, out NSRange effectiveRange);

		[Export ("rangeOfTextBlock:atIndex:")]
		NSRange GetRange (NSTextBlock textBlock, nuint index);

		[Export ("rangeOfTextTable:atIndex:")]
		NSRange GetRange (NSTextTable textTable, nuint index);

		[Export ("rangeOfTextList:atIndex:")]
		NSRange GetRange (NSTextList textList, nuint index);

		[Export ("itemNumberInTextList:atIndex:")]
		nint GetItemNumber (NSTextList textList, nuint index);

		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetData (NSRange range, [NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("this.GetData (range, options == null ? null : options.Dictionary, out error)")]
		NSData GetData (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapper (NSRange range, [NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("this.GetFileWrapper (range, options == null ? null : options.Dictionary, out error)")]
		NSFileWrapper GetFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

		[Export ("RTFFromRange:documentAttributes:")]
		NSData GetRtf (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetRtf (range, options == null ? null : options.Dictionary)")]
		NSData GetRtf (NSRange range, NSAttributedStringDocumentAttributes options);

		[Export ("RTFDFromRange:documentAttributes:")]
		NSData GetRtfd (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetRtfd (range, options == null ? null : options.Dictionary)")]
		NSData GetRtfd (NSRange range, NSAttributedStringDocumentAttributes options);

		[Export ("RTFDFileWrapperFromRange:documentAttributes:")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetRtfdFileWrapper (range, options == null ? null : options.Dictionary)")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options);

		[Export ("docFormatFromRange:documentAttributes:")]
		NSData GetDocFormat (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetDocFormat (range, options == null ? null : options.Dictionary)")]
		NSData GetDocFormat (NSRange range, NSAttributedStringDocumentAttributes options);
#else
		[Since (6,0)]
		[Export ("size")]
		CGSize Size { get; }

		[Since (6,0)]
		[Export ("drawAtPoint:")]
		void DrawString (CGPoint point);

		[Since (6,0)]
		[Export ("drawInRect:")]
		void DrawString (CGRect rect);

		[Since (6,0)]
		[Export ("drawWithRect:options:context:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);

		[Since (6,0)]
		[Export ("boundingRectWithSize:options:context:")]
		CGRect GetBoundingRect (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);
#endif

		// -(BOOL)containsAttachmentsInRange:(NSRange)range __attribute__((availability(macosx, introduced=10.11)));
		[Mac (10,11)][iOS (9,0)]
		[Export ("containsAttachmentsInRange:")]
		bool ContainsAttachmentsInRange (NSRange range);
	}

	[BaseType (typeof (NSObject),
		   Delegates=new string [] { "WeakDelegate" },
		   Events=new Type [] { typeof (NSCacheDelegate)} )]
	[Since (4,0)]
	public interface NSCache {
		[Export ("objectForKey:")]
		NSObject ObjectForKey (NSObject key);

		[Export ("setObject:forKey:")]
		void SetObjectforKey (NSObject obj, NSObject key);

		[Export ("setObject:forKey:cost:")]
		void SetCost (NSObject obj, NSObject key, nuint cost);

		[Export ("removeObjectForKey:")]
		void RemoveObjectForKey (NSObject key);

		[Export ("removeAllObjects")]
		void RemoveAllObjects ();

		//Detected properties
		[Export ("name")]
		string Name { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSCacheDelegate Delegate { get; set; }

		[Export ("totalCostLimit")]
		nuint TotalCostLimit { get; set; }

		[Export ("countLimit")]
		nuint CountLimit { get; set; }

		[Export ("evictsObjectsWithDiscardedContent")]
		bool EvictsObjectsWithDiscardedContent { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSCacheDelegate {
		[Export ("cache:willEvictObject:"), EventArgs ("NSObject")]
		void WillEvictObject (NSCache cache, NSObject obj);
	}

	[BaseType (typeof (NSObject), Name="NSCachedURLResponse")]
	// instance created with 'init' will crash when Dispose is called
	[DisableDefaultCtor]
	public interface NSCachedUrlResponse : NSCoding, NSSecureCoding, NSCopying {
		[Export ("initWithResponse:data:userInfo:storagePolicy:")]
		IntPtr Constructor (NSUrlResponse response, NSData data, [NullAllowed] NSDictionary userInfo, NSUrlCacheStoragePolicy storagePolicy);

		[Export ("initWithResponse:data:")]
		IntPtr Constructor (NSUrlResponse response, NSData data);
          
		[Export ("response")]
		NSUrlResponse Response { get; }

		[Export ("data")]
		NSData Data { get; }

		[Export ("userInfo")]
		NSDictionary UserInfo { get; }

		[Export ("storagePolicy")]
		NSUrlCacheStoragePolicy StoragePolicy { get; }
	}
	
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL - `init` now marked as NS_UNAVAILABLE
	[DisableDefaultCtor]
	public interface NSCalendar : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithCalendarIdentifier:")]
		IntPtr Constructor (NSString identifier);

		[Export ("calendarIdentifier")]
		string Identifier { get; }

		[Export ("currentCalendar")] [Static]
		NSCalendar CurrentCalendar { get; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; } 

		[Export ("firstWeekday")]
		nuint FirstWeekDay { get; set; } 

		[Export ("minimumDaysInFirstWeek")]
		nuint MinimumDaysInFirstWeek { get; set; }

		[Export ("components:fromDate:")]
		NSDateComponents Components (NSCalendarUnit unitFlags, NSDate fromDate);

		[Export ("components:fromDate:toDate:options:")]
		NSDateComponents Components (NSCalendarUnit unitFlags, NSDate fromDate, NSDate toDate, NSCalendarOptions opts);

		[Wrap ("Components (unitFlags, fromDate, toDate, (NSCalendarOptions) opts)")]
		NSDateComponents Components (NSCalendarUnit unitFlags, NSDate fromDate, NSDate toDate, NSDateComponentsWrappingBehavior opts);

		[Export ("dateByAddingComponents:toDate:options:")]
		NSDate DateByAddingComponents (NSDateComponents comps, NSDate date, NSCalendarOptions opts);

		[Wrap ("DateByAddingComponents (comps, date, (NSCalendarOptions) opts)")]
		NSDate DateByAddingComponents (NSDateComponents comps, NSDate date, NSDateComponentsWrappingBehavior opts);

		[Export ("dateFromComponents:")]
		NSDate DateFromComponents (NSDateComponents comps);

		[Field ("NSCalendarIdentifierGregorian"), Internal]
		NSString NSGregorianCalendar { get; }
		
		[Field ("NSCalendarIdentifierBuddhist"), Internal]
		NSString NSBuddhistCalendar { get; }
		
		[Field ("NSCalendarIdentifierChinese"), Internal]
		NSString NSChineseCalendar { get; }
		
		[Field ("NSCalendarIdentifierHebrew"), Internal]
		NSString NSHebrewCalendar { get; }
		
		[Field ("NSIslamicCalendar"), Internal]
		NSString NSIslamicCalendar { get; }
		
		[Field ("NSCalendarIdentifierIslamicCivil"), Internal]
		NSString NSIslamicCivilCalendar { get; }
		
		[Field ("NSCalendarIdentifierJapanese"), Internal]
		NSString NSJapaneseCalendar { get; }
		
		[Field ("NSCalendarIdentifierRepublicOfChina"), Internal]
		NSString NSRepublicOfChinaCalendar { get; }
		
		[Field ("NSCalendarIdentifierPersian"), Internal]
		NSString NSPersianCalendar { get; }
		
		[Field ("NSCalendarIdentifierIndian"), Internal]
		NSString NSIndianCalendar { get; }
		
		[Field ("NSCalendarIdentifierISO8601"), Internal]
		NSString NSISO8601Calendar { get; }

		[Field ("NSCalendarIdentifierCoptic"), Internal]
		NSString CopticCalendar { get; }

		[Field ("NSCalendarIdentifierEthiopicAmeteAlem"), Internal]
		NSString EthiopicAmeteAlemCalendar { get; }

		[Field ("NSCalendarIdentifierEthiopicAmeteMihret"), Internal]
		NSString EthiopicAmeteMihretCalendar { get; }

		[Mac(10,10)][iOS(8,0)]
		[Field ("NSCalendarIdentifierIslamicTabular"), Internal]
		NSString IslamicTabularCalendar { get; }

		[Mac(10,10)][iOS(8,0)]
		[Field ("NSCalendarIdentifierIslamicUmmAlQura"), Internal]
		NSString IslamicUmmAlQuraCalendar { get; }

		[Export ("eraSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] EraSymbols { get; }

		[Export ("longEraSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] LongEraSymbols { get; }

		[Export ("monthSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] MonthSymbols { get; }

		[Export ("shortMonthSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] ShortMonthSymbols { get; }

		[Export ("veryShortMonthSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] VeryShortMonthSymbols { get; }

		[Export ("standaloneMonthSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] StandaloneMonthSymbols { get; }

		[Export ("shortStandaloneMonthSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] ShortStandaloneMonthSymbols { get; }

		[Export ("veryShortStandaloneMonthSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] VeryShortStandaloneMonthSymbols { get; }

		[Export ("weekdaySymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] WeekdaySymbols { get; }

		[Export ("shortWeekdaySymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] ShortWeekdaySymbols { get; }

		[Export ("veryShortWeekdaySymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] VeryShortWeekdaySymbols { get; }

		[Export ("standaloneWeekdaySymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] StandaloneWeekdaySymbols { get; }

		[Export ("shortStandaloneWeekdaySymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] ShortStandaloneWeekdaySymbols { get; }

		[Export ("veryShortStandaloneWeekdaySymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] VeryShortStandaloneWeekdaySymbols { get; }

		[Export ("quarterSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] QuarterSymbols { get; }

		[Export ("shortQuarterSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] ShortQuarterSymbols { get; }

		[Export ("standaloneQuarterSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] StandaloneQuarterSymbols { get; }

		[Export ("shortStandaloneQuarterSymbols")]
		[Mac(10,7)][iOS(5,0)]
		string [] ShortStandaloneQuarterSymbols { get; }

		[Export ("AMSymbol")]
		[Mac(10,7)][iOS(5,0)]
		string AMSymbol { get; }

		[Export ("PMSymbol")]
		[Mac(10,7)][iOS(5,0)]
		string PMSymbol { get; }

		[Export ("compareDate:toDate:toUnitGranularity:")]
		[Mac(10,9)][iOS(8,0)]
		NSComparisonResult CompareDate(NSDate date1, NSDate date2, NSCalendarUnit granularity);

		[Export ("component:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		nint GetComponentFromDate (NSCalendarUnit unit, NSDate date);

		[Export ("components:fromDateComponents:toDateComponents:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDateComponents ComponentsFromDateToDate (NSCalendarUnit unitFlags, NSDateComponents startingDate, NSDateComponents resultDate, NSCalendarOptions options);

		[Export ("componentsInTimeZone:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		NSDateComponents ComponentsInTimeZone (NSTimeZone timezone, NSDate date);

		[Export ("date:matchesComponents:")]
		[Mac(10,9)][iOS(8,0)]
		bool Matches (NSDate date, NSDateComponents components);

		[Export ("dateByAddingUnit:value:toDate:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateByAddingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		[Export ("dateBySettingHour:minute:second:ofDate:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateBySettingsHour (nint hour, nint minute, nint second, NSDate date, NSCalendarOptions options);

		[Export ("dateBySettingUnit:value:ofDate:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateBySettingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		[Export ("dateWithEra:year:month:day:hour:minute:second:nanosecond:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate Date (nint era, nint year, nint month, nint date, nint hour, nint minute, nint second, nint nanosecond);

		[Export ("dateWithEra:yearForWeekOfYear:weekOfYear:weekday:hour:minute:second:nanosecond:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateForWeekOfYear (nint era, nint year, nint week, nint weekday, nint hour, nint minute, nint second, nint nanosecond);

		[Export ("enumerateDatesStartingAfterDate:matchingComponents:options:usingBlock:")]
		[Mac(10,9)][iOS(8,0)]
		void EnumerateDatesStartingAfterDate (NSDate start, NSDateComponents matchingComponents, NSCalendarOptions options, [BlockCallback] EnumerateDatesCallback callback);

		[Export ("getEra:year:month:day:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		void GetComponentsFromDate (out nint era, out nint year, out nint month, out nint day, NSDate date);

		[Export ("getEra:yearForWeekOfYear:weekOfYear:weekday:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		void GetComponentsFromDateForWeekOfYear (out nint era, out nint year, out nint weekOfYear, out nint weekday, NSDate date);

		[Export ("getHour:minute:second:nanosecond:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		void GetHourComponentsFromDate (out nint hour, out nint minute, out nint second, out nint nanosecond, NSDate date);

		[Export ("isDate:equalToDate:toUnitGranularity:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsEqualToUnitGranularity (NSDate date1, NSDate date2, NSCalendarUnit unit);

		[Export ("isDate:inSameDayAsDate:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsInSameDay (NSDate date1, NSDate date2);

		[Export ("isDateInToday:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInToday (NSDate date);

		[Export ("isDateInTomorrow:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInTomorrow (NSDate date);

		[Export ("isDateInWeekend:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInWeekend (NSDate date);

		[Export ("isDateInYesterday:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInYesterday (NSDate date);

		[Export ("nextDateAfterDate:matchingComponents:options:")]
		[Mac(10,9)][iOS(8,0)]
		[MarshalNativeExceptions]
		NSDate FindNextDateAfterDateMatching (NSDate date, NSDateComponents components, NSCalendarOptions options);

		[Export ("nextDateAfterDate:matchingHour:minute:second:options:")]
		[Mac(10,9)][iOS(8,0)]
		[MarshalNativeExceptions]
		NSDate FindNextDateAfterDateMatching (NSDate date, nint hour, nint minute, nint second, NSCalendarOptions options);

		[Export ("nextDateAfterDate:matchingUnit:value:options:")]
		[Mac(10,9)][iOS(8,0)]
		[MarshalNativeExceptions]
		NSDate FindNextDateAfterDateMatching (NSDate date, NSCalendarUnit unit, nint value, NSCalendarOptions options);

		[Export ("nextWeekendStartDate:interval:options:afterDate:")]
		[Mac(10,9)][iOS(8,0)]
		bool FindNextWeekend (out NSDate date, out double /* NSTimeInterval */ interval, NSCalendarOptions options, NSDate afterDate);

		[Export ("rangeOfWeekendStartDate:interval:containingDate:")]
		[Mac(10,9)][iOS(8,0)]
		bool RangeOfWeekendContainingDate (out NSDate weekendStartDate, out double /* NSTimeInterval */ interval, NSDate date);
		
		// although the ideal would be to use GetRange, we already have the method
		// RangeOfWeekendContainingDate and for the sake of consistency we are 
		// going to use the same name pattern.
		[Export ("minimumRangeOfUnit:")]
		NSRange MinimumRange (NSCalendarUnit unit);

		[Export ("maximumRangeOfUnit:")]
		NSRange MaximumRange (NSCalendarUnit unit);

		[Export ("rangeOfUnit:inUnit:forDate:")]
		NSRange Range (NSCalendarUnit smaller, NSCalendarUnit larger, NSDate date);

		[Export ("ordinalityOfUnit:inUnit:forDate:")]
		nuint Ordinality (NSCalendarUnit smaller, NSCalendarUnit larger, NSDate date);

		[Export ("rangeOfUnit:startDate:interval:forDate:")]
		bool Range (NSCalendarUnit unit, [NullAllowed] out NSDate datep, out double /* NSTimeInterval */ interval, NSDate date);

		[Export ("startOfDayForDate:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate StartOfDayForDate (NSDate date);

		[Mac(10,9)][iOS(8,0)]
		[Notification]
		[Field ("NSCalendarDayChangedNotification")]
		NSString DayChangedNotification { get; }
	}

#if MONOMAC
	// Obsolete, but the only API surfaced by WebKit.WebHistory.
	[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10, Message="Obsolete, use NSCalendar and NSDateComponents")]
	[BaseType (typeof (NSDate))]
	interface NSCalendarDate {
		[Export ("initWithString:calendarFormat:locale:")]
		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (string description, string calendarFormat, NSObject locale);

		[Export ("initWithString:calendarFormat:")]
		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (string description, string calendarFormat);

		[Export ("initWithString:")]
		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (string description);

		[Export ("initWithYear:month:day:hour:minute:second:timeZone:")]
		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (nint year, nuint month, nuint day, nuint hour, nuint minute, nuint second, NSTimeZone aTimeZone);

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("dateByAddingYears:months:days:hours:minutes:seconds:")]
		NSCalendarDate DateByAddingYears (nint year, nint month, nint day, nint hour, nint minute, nint second);

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfCommonEra")]
		nint DayOfCommonEra { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfMonth")]
		nint DayOfMonth { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfWeek")]
		nint DayOfWeek { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfYear")]
		nint DayOfYear { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("hourOfDay")]
		nint HourOfDay { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("minuteOfHour")]
		nint MinuteOfHour { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("monthOfYear")]
		nint MonthOfYear { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("secondOfMinute")]
		nint SecondOfMinute { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("yearOfCommonEra")]
		nint YearOfCommonEra { get; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("calendarFormat")]
		string CalendarFormat { get; set; }

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("descriptionWithCalendarFormat:locale:")]
		string GetDescription (string calendarFormat, NSObject locale);

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("descriptionWithCalendarFormat:")]
		string GetDescription (string calendarFormat);

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("descriptionWithLocale:")]
		string GetDescription (NSLocale locale);

		[Availability (Introduced = Platform.Mac_10_4, Deprecated = Platform.Mac_10_10)]
		[Export ("timeZone")]
		NSTimeZone TimeZone { get; set; }
	}
#endif

	[Since (3,2)]
	[BaseType (typeof (NSObject))]
	public interface NSCharacterSet : NSCoding, NSMutableCopying {
		[Static, Export ("alphanumericCharacterSet")]
		NSCharacterSet Alphanumerics {get;}

		[Static, Export ("capitalizedLetterCharacterSet")]
		NSCharacterSet Capitalized {get;}

		// TODO/FIXME: constructor?
		[Static, Export ("characterSetWithBitmapRepresentation:")]
		NSCharacterSet FromBitmap (NSData data);

		// TODO/FIXME: constructor?
		[Static, Export ("characterSetWithCharactersInString:")]
		NSCharacterSet FromString (string aString);

		[Static, Export ("characterSetWithContentsOfFile:")]
		NSCharacterSet FromFile (string path);

		[Static, Export ("characterSetWithRange:")]
		NSCharacterSet FromRange (NSRange aRange);

		[Static, Export ("controlCharacterSet")]
		NSCharacterSet Controls {get;}

		[Static, Export ("decimalDigitCharacterSet")]
		NSCharacterSet DecimalDigits {get;}

		[Static, Export ("decomposableCharacterSet")]
		NSCharacterSet Decomposables {get;}

		[Static, Export ("illegalCharacterSet")]
		NSCharacterSet Illegals {get;}

		[Static, Export ("letterCharacterSet")]
		NSCharacterSet Letters {get;}

		[Static, Export ("lowercaseLetterCharacterSet")]
		NSCharacterSet LowercaseLetters {get;}

		[Static, Export ("newlineCharacterSet")]
		NSCharacterSet Newlines {get;}

		[Static, Export ("nonBaseCharacterSet")]
		NSCharacterSet Marks {get;}

		[Static, Export ("punctuationCharacterSet")]
		NSCharacterSet Punctuation {get;}

		[Static, Export ("symbolCharacterSet")]
		NSCharacterSet Symbols {get;}

		[Static, Export ("uppercaseLetterCharacterSet")]
		NSCharacterSet UppercaseLetters {get;}

		[Static, Export ("whitespaceAndNewlineCharacterSet")]
		NSCharacterSet WhitespaceAndNewlines {get;}

		[Static, Export ("whitespaceCharacterSet")]
		NSCharacterSet Whitespaces {get;}

		[Export ("bitmapRepresentation")]
		NSData GetBitmapRepresentation ();

		[Export ("characterIsMember:")]
		bool Contains (char aCharacter);

		[Export ("hasMemberInPlane:")]
		bool HasMemberInPlane (byte thePlane);

		[Export ("invertedSet")]
		NSCharacterSet InvertedSet {get;}

		[Export ("isSupersetOfSet:")]
		bool IsSupersetOf (NSCharacterSet theOtherSet);

		[Export ("longCharacterIsMember:")]
		bool Contains (uint /* UTF32Char = UInt32 */ theLongChar);
	}

	[iOS (8,0), Mac(10,10)]
	[BaseType (typeof (NSFormatter))]
	interface NSMassFormatter {
		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("unitStyle")]
		NSFormattingUnitStyle UnitStyle { get; set; }

		[Export ("forPersonMassUse")]
		bool ForPersonMassUse { [Bind ("isForPersonMassUse")] get; set; }

		[Export ("stringFromValue:unit:")]
		string StringFromValue (double value, NSMassFormatterUnit unit);

		[Export ("stringFromKilograms:")]
		string StringFromKilograms (double numberInKilograms);

		[Export ("unitStringFromValue:unit:")]
		string UnitStringFromValue (double value, NSMassFormatterUnit unit);

		[Export ("unitStringFromKilograms:usedUnit:")]
		string UnitStringFromKilograms (double numberInKilograms, ref NSMassFormatterUnit unitp);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);
	}
	
#if !MONOMAC

	// Already exists in MonoMac: from from foundation-desktop?
	
	[BaseType (typeof (NSCharacterSet))]
	public interface NSMutableCharacterSet {
		[Export ("addCharactersInRange:")]
		void AddCharacters (NSRange aRange);
		
		[Export ("removeCharactersInRange:")]
		void RemoveCharacters (NSRange aRange);
		
		[Export ("addCharactersInString:")]
		void AddCharacters (NSString str);
		
		[Export ("removeCharactersInString:")]
		void RemoveCharacters (NSString str);
		
		[Export ("formUnionWithCharacterSet:")]
		void UnionWith (NSCharacterSet otherSet);
		
		[Export ("formIntersectionWithCharacterSet:")]
		void IntersectWith (NSCharacterSet otherSet);
		
		[Export ("invert")]
		void Invert ();

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("alphanumericCharacterSet")]
		NSCharacterSet Alphanumerics {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("capitalizedLetterCharacterSet")]
		NSCharacterSet Capitalized {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithBitmapRepresentation:")]
		NSCharacterSet FromBitmapRepresentation (NSData data);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithCharactersInString:")]
		NSCharacterSet FromString (string aString);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithContentsOfFile:")]
		NSCharacterSet FromFile (string path);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithRange:")]
		NSCharacterSet FromRange (NSRange aRange);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("controlCharacterSet")]
		NSCharacterSet Controls {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("decimalDigitCharacterSet")]
		NSCharacterSet DecimalDigits {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("decomposableCharacterSet")]
		NSCharacterSet Decomposables {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("illegalCharacterSet")]
		NSCharacterSet Illegals {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("letterCharacterSet")]
		NSCharacterSet Letters {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("lowercaseLetterCharacterSet")]
		NSCharacterSet LowercaseLetters {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("newlineCharacterSet")]
		NSCharacterSet Newlines {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("nonBaseCharacterSet")]
		NSCharacterSet Marks {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("punctuationCharacterSet")]
		NSCharacterSet Punctuation {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("symbolCharacterSet")]
		NSCharacterSet Symbols {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("uppercaseLetterCharacterSet")]
		NSCharacterSet UppercaseLetters {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("whitespaceAndNewlineCharacterSet")]
		NSCharacterSet WhitespaceAndNewlines {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("whitespaceCharacterSet")]
		NSCharacterSet Whitespaces {get;}
	}
#endif
	
	[BaseType (typeof (NSObject))]
	public interface NSCoder {

		//
		// Encoding and decoding
		//
		[Export ("encodeObject:")]
		void Encode ([NullAllowed] NSObject obj);
		
		[Export ("encodeRootObject:")]
		void EncodeRoot ([NullAllowed] NSObject obj);

		[Export ("decodeObject")]
		NSObject DecodeObject ();

		//
		// Encoding and decoding with keys
		// 
		[Export ("encodeConditionalObject:forKey:")]
		void EncodeConditionalObject ([NullAllowed] NSObject val, string key);
		
		[Export ("encodeObject:forKey:")]
		void Encode ([NullAllowed] NSObject val, string key);
		
		[Export ("encodeBool:forKey:")]
		void Encode (bool val, string key);
		
		[Export ("encodeDouble:forKey:")]
		void Encode (double val, string key);
		
		[Export ("encodeFloat:forKey:")]
		void Encode (float /* float, not CGFloat */ val, string key);
		
		[Export ("encodeInt32:forKey:")]
		void Encode (int /* int32 */ val, string key);
		
		[Export ("encodeInt64:forKey:")]
		void Encode (long val, string key);

#if XAMCORE_2_0
		[Export ("encodeInteger:forKey:")]
		void Encode (nint val, string key);
#endif

		[Export ("encodeBytes:length:forKey:")]
		void EncodeBlock (IntPtr bytes, nint length, string key);

		[Export ("containsValueForKey:")]
		bool ContainsKey (string key);
		
		[Export ("decodeBoolForKey:")]
		bool DecodeBool (string key);

		[Export ("decodeDoubleForKey:")]
		double DecodeDouble (string key);

		[Export ("decodeFloatForKey:")]
		float DecodeFloat (string key); /* float, not CGFloat */ 

		[Export ("decodeInt32ForKey:")]
		int DecodeInt (string key); /* int, not NSInteger */

		[Export ("decodeInt64ForKey:")]
		long DecodeLong (string key);

#if XAMCORE_2_0
		[Export ("decodeIntegerForKey:")]
		nint DecodeNInt (string key);
#endif

		[Export ("decodeObjectForKey:")]
		NSObject DecodeObject (string key);

		[Export ("decodeBytesForKey:returnedLength:")]
		IntPtr DecodeBytes (string key, out nuint length);

		[Export ("decodeBytesWithReturnedLength:")]
		IntPtr DecodeBytes (out nuint length);

		[Since (6,0)]
		[Export ("allowedClasses")]
		NSSet AllowedClasses { get; }

		[Since (6,0)]
		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding ();

		[Since (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectAndReturnError:")]
		NSObject DecodeTopLevelObject (out NSError error);

		[Since (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectForKey:error:")]
		NSObject DecodeTopLevelObject (string key, out NSError error);

		[Since (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectOfClass:forKey:error:")]
		NSObject DecodeTopLevelObject (Class klass, string key, out NSError error);

		[Since (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectOfClasses:forKey:error:")]
		NSObject DecodeTopLevelObject ([NullAllowed] NSSet<Class> setOfClasses, string key, out NSError error);

		[Since (9,0), Mac (10,11)]
		[Export ("failWithError:")]
		void Fail (NSError error);

		[Export ("systemVersion")]
		uint SystemVersion { get; }
	}
	
	[BaseType (typeof (NSPredicate))]
	public interface NSComparisonPredicate : NSSecureCoding {
		[Static, Export ("predicateWithLeftExpression:rightExpression:modifier:type:options:")]
		NSComparisonPredicate Create (NSExpression leftExpression, NSExpression rightExpression, NSComparisonPredicateModifier comparisonModifier, NSPredicateOperatorType operatorType, NSComparisonPredicateOptions comparisonOptions);

		[Static, Export ("predicateWithLeftExpression:rightExpression:customSelector:")]
		NSComparisonPredicate FromSelector (NSExpression leftExpression, NSExpression rightExpression, Selector selector);

		[DesignatedInitializer]
		[Export ("initWithLeftExpression:rightExpression:modifier:type:options:")]
		IntPtr Constructor (NSExpression leftExpression, NSExpression rightExpression, NSComparisonPredicateModifier comparisonModifier, NSPredicateOperatorType operatorType, NSComparisonPredicateOptions comparisonOptions);
		
		[DesignatedInitializer]
		[Export ("initWithLeftExpression:rightExpression:customSelector:")]
		IntPtr Constructor (NSExpression leftExpression, NSExpression rightExpression, Selector selector);

		[Export ("predicateOperatorType")]
		NSPredicateOperatorType PredicateOperatorType { get; }

		[Export ("comparisonPredicateModifier")]
		NSComparisonPredicateModifier ComparisonPredicateModifier { get; }

		[Export ("leftExpression")]
		NSExpression LeftExpression { get; }

		[Export ("rightExpression")]
		NSExpression RightExpression { get; }

		[Export ("customSelector")]
		Selector CustomSelector { get; }

		[Export ("options")]
		NSComparisonPredicateOptions Options { get; }
	}

	[BaseType (typeof (NSPredicate))]
	[DisableDefaultCtor] // An uncaught exception was raised: Can't have a NOT predicate with no subpredicate.
	public interface NSCompoundPredicate : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithType:subpredicates:")]
		IntPtr Constructor (NSCompoundPredicateType type, NSPredicate[] subpredicates);

		[Export ("compoundPredicateType")]
		NSCompoundPredicateType Type { get; }

		[Export ("subpredicates")]
		NSPredicate[] Subpredicates { get; } 

		[Static]
		[Export ("andPredicateWithSubpredicates:")]
		NSCompoundPredicate CreateAndPredicate (NSPredicate[] subpredicates);

		[Static]
		[Export ("orPredicateWithSubpredicates:")]
		NSCompoundPredicate CreateOrPredicate (NSPredicate [] subpredicates);

		[Static]
		[Export ("notPredicateWithSubpredicate:")]
		NSCompoundPredicate CreateNotPredicate (NSPredicate predicate);

	}

	public delegate void NSDataByteRangeEnumerator (IntPtr bytes, NSRange range, ref bool stop);
	
	[BaseType (typeof (NSObject))]
	public interface NSData : NSSecureCoding, NSMutableCopying {
		[Export ("dataWithContentsOfURL:")]
		[Static]
		NSData FromUrl (NSUrl url);

		[Export ("dataWithContentsOfURL:options:error:")]
		[Static]
		NSData FromUrl (NSUrl url, NSDataReadingOptions mask, out NSError error);

		[Export ("dataWithContentsOfFile:")][Static]
		NSData FromFile (string path);
		
		[Export ("dataWithContentsOfFile:options:error:")]
		[Static]
		NSData FromFile (string path, NSDataReadingOptions mask, out NSError error);

		[Export ("dataWithData:")]
		[Static]
		NSData FromData (NSData source);

		[Export ("dataWithBytes:length:"), Static]
		NSData FromBytes (IntPtr bytes, nuint size);

		[Export ("dataWithBytesNoCopy:length:"), Static]
		NSData FromBytesNoCopy (IntPtr bytes, nuint size);

		[Export ("dataWithBytesNoCopy:length:freeWhenDone:"), Static]
		NSData FromBytesNoCopy (IntPtr bytes, nuint size, bool freeWhenDone);

		[Export ("bytes")]
		IntPtr Bytes { get; }

		[Export ("length")]
		nuint Length { get; [NotImplemented ("Not available on NSData, only available on NSMutableData")] set; }

		[Export ("writeToFile:options:error:")]
#if XAMCORE_2_0
		[Internal]
#endif
		bool _Save (string file, nint options, IntPtr addr);
		
		[Export ("writeToURL:options:error:")]
#if XAMCORE_2_0
		[Internal]
#endif
		bool _Save (NSUrl url, nint options, IntPtr addr);

		[Export ("writeToFile:atomically:")]
		bool Save (string path, bool atomically);

		[Export ("writeToURL:atomically:")]
		bool Save (NSUrl url, bool atomically);

		[Export ("subdataWithRange:")]
		NSData Subdata (NSRange range);

		[Export ("getBytes:length:")]
		void GetBytes (IntPtr buffer, nuint length);

		[Export ("getBytes:range:")]
		void GetBytes (IntPtr buffer, NSRange range);

		[Export ("rangeOfData:options:range:")]
		[Since (4,0)]
		NSRange Find (NSData dataToFind, NSDataSearchOptions searchOptions, NSRange searchRange);

		[Since (7,0), Mavericks] // 10.9
		[Export ("initWithBase64EncodedString:options:")]
		IntPtr Constructor (string base64String, NSDataBase64DecodingOptions options);

		[Since (7,0), Mavericks] // 10.9
		[Export ("initWithBase64EncodedData:options:")]
		IntPtr Constructor (NSData base64Data, NSDataBase64DecodingOptions options);

		[Since (7,0), Mavericks] // 10.9
		[Export ("base64EncodedDataWithOptions:")]
		NSData GetBase64EncodedData (NSDataBase64EncodingOptions options);

		[Since (7,0), Mavericks] // 10.9
		[Export ("base64EncodedStringWithOptions:")]
		string GetBase64EncodedString (NSDataBase64EncodingOptions options);

		[Since (7,0), Mavericks]
		[Export ("enumerateByteRangesUsingBlock:")]
		void EnumerateByteRange (NSDataByteRangeEnumerator enumerator);

		[Since (7,0), Mavericks]
		[Export ("initWithBytesNoCopy:length:deallocator:")]
		IntPtr Constructor (IntPtr bytes, nuint length, Action<IntPtr,nuint> deallocator);
	}

	[BaseType (typeof (NSRegularExpression))]
	public interface NSDataDetector : NSCopying, NSCoding {
		// Invalid parent ctor: -[NSDataDetector initWithPattern:options:error:]: Not valid for NSDataDetector
//		[Export ("initWithPattern:options:error:")]
//		IntPtr Constructor (NSString pattern, NSRegularExpressionOptions options, out NSError error);

		[Export ("dataDetectorWithTypes:error:"), Static]
		NSDataDetector Create (NSTextCheckingTypes checkingTypes, out NSError error);

		[Export ("checkingTypes")]
		NSTextCheckingTypes CheckingTypes { get; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSDateComponents : NSSecureCoding, NSCopying, INSCopying, INSSecureCoding, INativeObject {
		[Since (4,0)]
		[NullAllowed] // by default this property is null
		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("calendar", ArgumentSemantic.Copy)]
		[Since (4,0)]
		NSCalendar Calendar { get; set; }

		[Export ("quarter")]
		[Since (4,0)]
		nint Quarter { get; set; }

		[Export ("date")]
		[Since (4,0)]
		NSDate Date { get; }

		//Detected properties
		[Export ("era")]
		nint Era { get; set; }

		[Export ("year")]
		nint Year { get; set; }

		[Export ("month")]
		nint Month { get; set; }

		[Export ("day")]
		nint Day { get; set; }

		[Export ("hour")]
		nint Hour { get; set; }

		[Export ("minute")]
		nint Minute { get; set; }

		[Export ("second")]
		nint Second { get; set; }

		[Export ("nanosecond")]
		[Mac(10,7)][iOS(5,0)]
		nint Nanosecond { get; set; }

		[Export ("week")]
		[Availability (Introduced = Platform.Mac_10_4 | Platform.iOS_2_0, Deprecated = Platform.Mac_10_9 | Platform.iOS_7_0, Message = "Use WeekOfMonth or WeekOfYear, depending on which you mean")]
		nint Week { get; set; }

		[Export ("weekday")]
		nint Weekday { get; set; }

		[Export ("weekdayOrdinal")]
		nint WeekdayOrdinal { get; set; }

		[Mac(10,7)][iOS(5,0)]
		[Export ("weekOfMonth")]
		nint WeekOfMonth { get; set; }

		[Mac(10,7)][iOS(5,0)]
		[Export ("weekOfYear")]
		nint WeekOfYear { get; set; }
		
		[Mac(10,7)][iOS(5,0)]
		[Export ("yearForWeekOfYear")]
		nint YearForWeekOfYear { get; set; }

		[Mac(10,8)][iOS(6,0)]
		[Export ("leapMonth")]
		bool IsLeapMonth { [Bind ("isLeapMonth")] get; set; }

		[Export ("isValidDate")]
		[Mac(10,9)][iOS(8,0)]
		bool IsValidDate { get; }

		[Export ("isValidDateInCalendar:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsValidDateInCalendar (NSCalendar calendar);

		[Export ("setValue:forComponent:")]
		[Mac(10,9)][iOS(8,0)]
		void SetValueForComponent (nint value, NSCalendarUnit unit);

		[Export ("valueForComponent:")]
		[Mac(10,9)][iOS(8,0)]
		nint GetValueForComponent (NSCalendarUnit unit);
	}
	
	[Since (6,0)]
	[BaseType (typeof (NSFormatter))]
	interface NSByteCountFormatter {
		[Export ("allowsNonnumericFormatting")]
		bool AllowsNonnumericFormatting { get; set; }

		[Export ("includesUnit")]
		bool IncludesUnit { get; set; }

		[Export ("includesCount")]
		bool IncludesCount { get; set; }

		[Export ("includesActualByteCount")]
		bool IncludesActualByteCount { get; set; }
		
		[Export ("adaptive")]
		bool Adaptive { [Bind ("isAdaptive")] get; set;  }

		[Export ("zeroPadsFractionDigits")]
		bool ZeroPadsFractionDigits { get; set;  }

		[Static]
		[Export ("stringFromByteCount:countStyle:")]
		string Format (long byteCount, NSByteCountFormatterCountStyle countStyle);

		[Export ("stringFromByteCount:")]
		string Format (long byteCount);

		[Export ("allowedUnits")]
		NSByteCountFormatterUnits AllowedUnits { get; set; }

		[Export ("countStyle")]
		NSByteCountFormatterCountStyle CountStyle { get; set; }

		[iOS (8,0), Mac(10,10)]
		[Export ("formattingContext")]
		NSFormattingContext FormattingContext { get; set; }
	}

	[BaseType (typeof (NSFormatter))]
	public interface NSDateFormatter {
		[Export ("stringFromDate:")]
		string ToString (NSDate date);

		[Export ("dateFromString:")]
		NSDate Parse (string date);

		[Export ("dateFormat")]
		string DateFormat { get; set; }

		[Export ("dateStyle")]
		NSDateFormatterStyle DateStyle { get; set; }

		[Export ("timeStyle")]
		NSDateFormatterStyle TimeStyle { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("generatesCalendarDates")]
		bool GeneratesCalendarDates { get; set; }

		[Export ("formatterBehavior")]
		NSDateFormatterBehavior Behavior { get; set; }

		[Export ("defaultFormatterBehavior"), Static]
		NSDateFormatterBehavior DefaultBehavior { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		// not exposed as a property in documentation
		[Export ("isLenient")]
		bool IsLenient { get; [Bind ("setLenient:")] set; } 

		[Export ("twoDigitStartDate", ArgumentSemantic.Copy)]
		NSDate TwoDigitStartDate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("defaultDate", ArgumentSemantic.Copy)]
		NSDate DefaultDate { get; set; }

		[Export ("eraSymbols")]
		string [] EraSymbols { get; set; }

		[Export ("monthSymbols")]
		string [] MonthSymbols { get; set; }

		[Export ("shortMonthSymbols")]
		string [] ShortMonthSymbols { get; set; }

		[Export ("weekdaySymbols")]
		string [] WeekdaySymbols { get; set; }

		[Export ("shortWeekdaySymbols")]
		string [] ShortWeekdaySymbols { get; set; } 

		[Export ("AMSymbol")]
		string AMSymbol { get; set; }

		[Export ("PMSymbol")]
		string PMSymbol { get; set; }

		[Export ("longEraSymbols")]
		string [] LongEraSymbols { get; set; }

		[Export ("veryShortMonthSymbols")]
		string [] VeryShortMonthSymbols { get; set; }
		
		[Export ("standaloneMonthSymbols")]
		string [] StandaloneMonthSymbols { get; set; }

		[Export ("shortStandaloneMonthSymbols")]
		string [] ShortStandaloneMonthSymbols { get; set; }

		[Export ("veryShortStandaloneMonthSymbols")]
		string [] VeryShortStandaloneMonthSymbols { get; set; }
		
		[Export ("veryShortWeekdaySymbols")]
		string [] VeryShortWeekdaySymbols { get; set; }

		[Export ("standaloneWeekdaySymbols")]
		string [] StandaloneWeekdaySymbols { get; set; }

		[Export ("shortStandaloneWeekdaySymbols")]
		string [] ShortStandaloneWeekdaySymbols { get; set; }
		
		[Export ("veryShortStandaloneWeekdaySymbols")]
		string [] VeryShortStandaloneWeekdaySymbols { get; set; }
		
		[Export ("quarterSymbols")]
		string [] QuarterSymbols { get; set; }

		[Export ("shortQuarterSymbols")]
		string [] ShortQuarterSymbols { get; set; }
		
		[Export ("standaloneQuarterSymbols")]
		string [] StandaloneQuarterSymbols { get; set; }

		[Export ("shortStandaloneQuarterSymbols")]
		string [] ShortStandaloneQuarterSymbols { get; set; }

		[Export ("gregorianStartDate", ArgumentSemantic.Copy)]
		NSDate GregorianStartDate { get; set; }

		[Export ("localizedStringFromDate:dateStyle:timeStyle:")]
		[Static]
		string ToLocalizedString (NSDate date, NSDateFormatterStyle dateStyle, NSDateFormatterStyle timeStyle);

		[Export ("dateFormatFromTemplate:options:locale:")]
		[Static]
		string GetDateFormatFromTemplate (string template, nuint options, NSLocale locale);

		[Export ("doesRelativeDateFormatting")]
		bool DoesRelativeDateFormatting { get; set; }

		[iOS (8,0), Mac (10,10)]
		[Export ("setLocalizedDateFormatFromTemplate:")]
		void SetLocalizedDateFormatFromTemplate (string dateFormatTemplate);		
	}

	[iOS (8,0)][Mac(10,10)]
	[BaseType (typeof (NSFormatter))]
	interface NSDateComponentsFormatter {
		[Export ("unitsStyle")]
		NSDateComponentsFormatterUnitsStyle UnitsStyle { get; set; }

		[Export ("allowedUnits")]
		NSCalendarUnit AllowedUnits { get; set; }

		[Export ("zeroFormattingBehavior")]
		NSDateComponentsFormatterZeroFormattingBehavior ZeroFormattingBehavior { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("allowsFractionalUnits")]
		bool AllowsFractionalUnits { get; set; }

		[Export ("maximumUnitCount")]
		nint MaximumUnitCount { get; set; }

		[Export ("collapsesLargestUnit")]
		bool CollapsesLargestUnit { get; set; }

		[Export ("includesApproximationPhrase")]
		bool IncludesApproximationPhrase { get; set; }

		[Export ("includesTimeRemainingPhrase")]
		bool IncludesTimeRemainingPhrase { get; set; }

		[Export ("formattingContext")]
		NSFormattingContext FormattingContext { get; set; }

		[Export ("stringForObjectValue:")]
		string StringForObjectValue (NSObject obj);

		[Export ("stringFromDateComponents:")]
		string StringFromDateComponents (NSDateComponents components);

		[Export ("stringFromDate:toDate:")]
		string StringFromDate (NSDate startDate, NSDate endDate);

		[Export ("stringFromTimeInterval:")]
		string StringFromTimeInterval (double ti);

		[Static, Export ("localizedStringFromDateComponents:unitsStyle:")]
		string LocalizedStringFromDateComponents (NSDateComponents components, NSDateComponentsFormatterUnitsStyle unitsStyle);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);
	}

	[iOS (8,0)][Mac(10,10)]
	[BaseType (typeof (NSFormatter))]
	interface NSDateIntervalFormatter {

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("dateTemplate")]
		string DateTemplate { get; set; }

		[Export ("dateStyle")]
		NSDateIntervalFormatterStyle DateStyle { get; set; }

		[Export ("timeStyle")]
		NSDateIntervalFormatterStyle TimeStyle { get; set; }

		[Export ("stringFromDate:toDate:")]
		string StringFromDate (NSDate fromDate, NSDate toDate);
	}

	[iOS (8,0)][Mac(10,10)]
	[BaseType (typeof (NSFormatter))]
	interface NSEnergyFormatter {
		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("unitStyle")]
		NSFormattingUnitStyle UnitStyle { get; set; }

		[Export ("forFoodEnergyUse")]
		bool ForFoodEnergyUse { [Bind ("isForFoodEnergyUse")] get; set; }

		[Export ("stringFromValue:unit:")]
		string StringFromValue (double value, NSEnergyFormatterUnit unit);

		[Export ("stringFromJoules:")]
		string StringFromJoules (double numberInJoules);

		[Export ("unitStringFromValue:unit:")]
		string UnitStringFromValue (double value, NSEnergyFormatterUnit unit);

		[Export ("unitStringFromJoules:usedUnit:")]
		string UnitStringFromJoules (double numberInJoules, out NSEnergyFormatterUnit unitp);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);
	}

#if !XAMCORE_2_0
	public delegate void NSFileHandleUpdateHandler (NSFileHandle handle);
#endif

	public interface NSFileHandleReadEventArgs {
		[Export ("NSFileHandleNotificationDataItem")]
		NSData AvailableData { get; }

		[Export ("NSFileHandleError", ArgumentSemantic.Assign)]
		nint UnixErrorCode { get; }
	}

	public interface NSFileHandleConnectionAcceptedEventArgs {
		[Export ("NSFileHandleNotificationFileHandleItem")]
		NSFileHandle NearSocketConnection { get; }
		
		[Export ("NSFileHandleError", ArgumentSemantic.Assign)]
		nint UnixErrorCode { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return invalid handle
	public interface NSFileHandle : NSSecureCoding {
		[Export ("availableData")]
		NSData AvailableData ();
		
		[Export ("readDataToEndOfFile")]
		NSData ReadDataToEndOfFile ();

		[Export ("readDataOfLength:")]
		NSData ReadDataOfLength (nuint length);

		[Export ("writeData:")]
		void WriteData (NSData data);

		[Export ("offsetInFile")]
		ulong OffsetInFile ();

		[Export ("seekToEndOfFile")]
		ulong SeekToEndOfFile ();

		[Export ("seekToFileOffset:")]
		void SeekToFileOffset (ulong offset);

		[Export ("truncateFileAtOffset:")]
		void TruncateFileAtOffset (ulong offset);

		[Export ("synchronizeFile")]
		void SynchronizeFile ();

		[Export ("closeFile")]
		void CloseFile ();
		
		[Static]
		[Export ("fileHandleWithStandardInput")]
		NSFileHandle FromStandardInput ();
		
		[Static]
		[Export ("fileHandleWithStandardOutput")]
		NSFileHandle FromStandardOutput ();

		[Static]
		[Export ("fileHandleWithStandardError")]
		NSFileHandle FromStandardError ();

		[Static]
		[Export ("fileHandleWithNullDevice")]
		NSFileHandle FromNullDevice ();

		[Static]
		[Export ("fileHandleForReadingAtPath:")]
		NSFileHandle OpenRead (string path);

		[Static]
		[Export ("fileHandleForWritingAtPath:")]
		NSFileHandle OpenWrite (string path);

		[Static]
		[Export ("fileHandleForUpdatingAtPath:")]
		NSFileHandle OpenUpdate (string path);

		[Static]
		[Export ("fileHandleForReadingFromURL:error:")]
		NSFileHandle OpenReadUrl (NSUrl url, out NSError error);

		[Static]
		[Export ("fileHandleForWritingToURL:error:")]
		NSFileHandle OpenWriteUrl (NSUrl url, out NSError error);

		[Static]
		[Export ("fileHandleForUpdatingURL:error:")]
		NSFileHandle OpenUpdateUrl (NSUrl url, out NSError error);
		
		[Export ("readInBackgroundAndNotifyForModes:")]
		void ReadInBackground (NSString [] notifyRunLoopModes);
		
		[Export ("readInBackgroundAndNotify")]
		void ReadInBackground ();

		[Export ("readToEndOfFileInBackgroundAndNotifyForModes:")]
		void ReadToEndOfFileInBackground (NSString [] notifyRunLoopModes);

		[Export ("readToEndOfFileInBackgroundAndNotify")]
		void ReadToEndOfFileInBackground ();

		[Export ("acceptConnectionInBackgroundAndNotifyForModes:")]
		void AcceptConnectionInBackground (NSString [] notifyRunLoopModes);

		[Export ("acceptConnectionInBackgroundAndNotify")]
		void AcceptConnectionInBackground ();

		[Export ("waitForDataInBackgroundAndNotifyForModes:")]
		void WaitForDataInBackground (NSString [] notifyRunLoopModes);

		[Export ("waitForDataInBackgroundAndNotify")]
		void WaitForDataInBackground ();
		
		[DesignatedInitializer]
		[Export ("initWithFileDescriptor:closeOnDealloc:")]
		IntPtr Constructor (int /* int, not NSInteger */ fd, bool closeOnDealloc);
		
		[Export ("initWithFileDescriptor:")]
		IntPtr Constructor (int /* int, not NSInteger */ fd);

		[Export ("fileDescriptor")]
		int FileDescriptor { get; } /* int, not NSInteger */

		[Since (5,0)]
		[Export ("setReadabilityHandler:")]
#if XAMCORE_2_0
		void SetReadabilityHandler ([NullAllowed] Action<NSFileHandle> readCallback);
#else
		void SetReadabilityHandler ([NullAllowed] NSFileHandleUpdateHandler readCallback);
#endif

		[Since (5,0)]
		[Export ("setWriteabilityHandler:")]
#if XAMCORE_2_0
		void SetWriteabilityHandle ([NullAllowed] Action<NSFileHandle> writeCallback);
#else
		void SetWriteabilityHandle ([NullAllowed] NSFileHandleUpdateHandler writeCallback);
#endif

		[Field ("NSFileHandleOperationException")]
		NSString OperationException { get; }

		[Field ("NSFileHandleReadCompletionNotification")]
		[Notification (typeof (NSFileHandleReadEventArgs))]
		NSString ReadCompletionNotification { get; }
		
		[Field ("NSFileHandleReadToEndOfFileCompletionNotification")]
		[Notification (typeof (NSFileHandleReadEventArgs))]
		NSString ReadToEndOfFileCompletionNotification { get; }
		
		[Field ("NSFileHandleConnectionAcceptedNotification")]
		[Notification (typeof (NSFileHandleConnectionAcceptedEventArgs))]
		NSString ConnectionAcceptedNotification { get; }

		[Field ("NSFileHandleDataAvailableNotification")]
		[Notification]
		NSString DataAvailableNotification { get; }
	}

	[iOS (9,0), Mac(10,11)]
	[Static]
	interface NSPersonNameComponent {
		[Field ("NSPersonNameComponentKey")]
		NSString ComponentKey { get; }
		
		[Field ("NSPersonNameComponentGivenName")]
		NSString GivenName { get; }
		
		[Field ("NSPersonNameComponentFamilyName")]
		NSString FamilyName { get; }
		
		[Field ("NSPersonNameComponentMiddleName")]
		NSString MiddleName { get; }
		
		[Field ("NSPersonNameComponentPrefix")]
		NSString Prefix { get; }
		
		[Field ("NSPersonNameComponentSuffix")]
		NSString Suffix { get; }
		
		[Field ("NSPersonNameComponentNickname")]
		NSString Nickname { get; }
		
		[Field ("NSPersonNameComponentDelimiter")]
		NSString Delimiter { get; }
	}
	

	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSObject))]
	interface NSPersonNameComponents : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("namePrefix")]
		string NamePrefix { get; set; }

		[NullAllowed, Export ("givenName")]
		string GivenName { get; set; }

		[NullAllowed, Export ("middleName")]
		string MiddleName { get; set; }

		[NullAllowed, Export ("familyName")]
		string FamilyName { get; set; }
	
		[NullAllowed, Export ("nameSuffix")]
		string NameSuffix { get; set; }
	
		[NullAllowed, Export ("nickname")]
		string Nickname { get; set; }
	
		[NullAllowed, Export ("phoneticRepresentation", ArgumentSemantic.Copy)]
		NSPersonNameComponents PhoneticRepresentation { get; set; }
	}

	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSFormatter))]
	interface NSPersonNameComponentsFormatter
	{
		[Export ("style", ArgumentSemantic.Assign)]
		NSPersonNameComponentsFormatterStyle Style { get; set; }
	
		[Export ("phonetic")]
		bool Phonetic { [Bind ("isPhonetic")] get; set; }
	
		[Static]
		[Export ("localizedStringFromPersonNameComponents:style:options:")]
		string GetLocalizedString (NSPersonNameComponents components, NSPersonNameComponentsFormatterStyle nameFormatStyle, NSPersonNameComponentsFormatterOptions nameOptions);
	
		[Export ("stringFromPersonNameComponents:")]
		string GetString (NSPersonNameComponents components);
	
		[Export ("annotatedStringFromPersonNameComponents:")]
		NSAttributedString GetAnnotatedString (NSPersonNameComponents components);
	
		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject result, string str, out string errorDescription);
	}
	
	
	[BaseType (typeof (NSObject))]
	public interface NSPipe {
		
		[Export ("fileHandleForReading")]
		NSFileHandle ReadHandle { get; }
		
		[Export ("fileHandleForWriting")]
		NSFileHandle WriteHandle { get; }

		[Static]
		[Export ("pipe")]
		NSPipe Create ();
	}

	[BaseType (typeof (NSObject))]
	public interface NSFormatter : NSCoding, NSCopying {
		[Export ("stringForObjectValue:")]
		string StringFor (NSObject value);

		// - (NSAttributedString *)attributedStringForObjectValue:(id)obj withDefaultAttributes:(NSDictionary *)attrs;

		[Export ("editingStringForObjectValue:")]
		string EditingStringFor (NSObject value);

#if XAMCORE_2_0
		[Internal]
		[Sealed]
		[Export ("attributedStringForObjectValue:withDefaultAttributes:")]
		NSAttributedString GetAttributedString (NSObject obj, NSDictionary defaultAttributes);
#endif

		// -(NSAttributedString *)attributedStringForObjectValue:(id)obj withDefaultAttributes:(NSDictionary *)attrs;
		[Export ("attributedStringForObjectValue:withDefaultAttributes:")]
		NSAttributedString GetAttributedString (NSObject obj, NSDictionary<NSString, NSObject> defaultAttributes);

		[Wrap ("GetAttributedString (obj, defaultAttributes == null ? null : defaultAttributes.Dictionary)")]
#if MONOMAC
		NSAttributedString GetAttributedString (NSObject obj, NSStringAttributes defaultAttributes);
#else
		NSAttributedString GetAttributedString (NSObject obj, UIStringAttributes defaultAttributes);
#endif

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out NSString error);

		[Export ("isPartialStringValid:newEditingString:errorDescription:")]
		bool IsPartialStringValid (string partialString, out string newString, out NSString error);

		[Export ("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
		unsafe bool IsPartialStringValid (out string partialString, out NSRange proposedSelRange, string origString, NSRange origSelRange, out NSString error);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSCoding {
#if XAMCORE_2_0
		// [Abstract]
		[Export ("initWithCoder:")]
		IntPtr Constructor (NSCoder decoder);

		[Abstract]
		[Export ("encodeWithCoder:")]
		void EncodeTo (NSCoder encoder);
#endif
	}

	[Protocol]
	public interface NSSecureCoding : NSCoding {
		// note: +supportsSecureCoding being static it is not a good "generated" binding candidate
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSCopying {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("copyWithZone:")]
		NSObject Copy ([NullAllowed] NSZone zone);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSMutableCopying : NSCopying {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("mutableCopyWithZone:")]
		[return: Release ()]
		NSObject MutableCopy ([NullAllowed] NSZone zone);
	}

	public interface INSMutableCopying {}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSKeyedArchiverDelegate {
		[Export ("archiver:didEncodeObject:"), EventArgs ("NSObject")]
		void EncodedObject (NSKeyedArchiver archiver, NSObject obj);
		
		[Export ("archiverDidFinish:")]
		void Finished (NSKeyedArchiver archiver);
		
		[Export ("archiver:willEncodeObject:"), DelegateName ("NSEncodeHook"), DefaultValue (null)]
		NSObject WillEncode (NSKeyedArchiver archiver, NSObject obj);
		
		[Export ("archiverWillFinish:")]
		void Finishing (NSKeyedArchiver archiver);
		
		[Export ("archiver:willReplaceObject:withObject:"), EventArgs ("NSArchiveReplace")]
		void ReplacingObject (NSKeyedArchiver archiver, NSObject oldObject, NSObject newObject);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSKeyedUnarchiverDelegate {
		[Export ("unarchiver:didDecodeObject:"), DelegateName ("NSDecoderCallback"), DefaultValue (null)]
		NSObject DecodedObject (NSKeyedUnarchiver unarchiver, NSObject obj);
		
		[Export ("unarchiverDidFinish:")]
		void Finished (NSKeyedUnarchiver unarchiver);
		
		[Export ("unarchiver:cannotDecodeObjectOfClassName:originalClasses:"), DelegateName ("NSDecoderHandler"), DefaultValue (null)]
		Class CannotDecodeClass (NSKeyedUnarchiver unarchiver, string klass, string [] classes);
		
		[Export ("unarchiverWillFinish:")]
		void Finishing (NSKeyedUnarchiver unarchiver);
		
		[Export ("unarchiver:willReplaceObject:withObject:"), EventArgs ("NSArchiveReplace")]
		void ReplacingObject (NSKeyedUnarchiver unarchiver, NSObject oldObject, NSObject newObject);
	}

	[BaseType (typeof (NSCoder),
		   Delegates=new string [] {"WeakDelegate"},
		   Events=new Type [] { typeof (NSKeyedArchiverDelegate) })]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSKeyedArchiver init]: cannot use -init for initialization
	[DisableDefaultCtor]
	public interface NSKeyedArchiver {
		[Export ("initForWritingWithMutableData:")]
		IntPtr Constructor (NSMutableData data);
	
		[Export ("archivedDataWithRootObject:")]
		[Static]
		NSData ArchivedDataWithRootObject (NSObject root);
		
		[Export ("archiveRootObject:toFile:")]
		[Static]
		bool ArchiveRootObjectToFile (NSObject root, string file);

		[Export ("finishEncoding")]
		void FinishEncoding ();

		[Export ("outputFormat")]
		NSPropertyListFormat PropertyListFormat { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSKeyedArchiverDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClassName:forClass:")]
		void SetClassName (string name, Class kls);

		[Export ("classNameForClass:")]
		string GetClassName (Class kls);

		[Since (7,0), Mavericks]
		[Field ("NSKeyedArchiveRootObjectKey")]
		NSString RootObjectKey { get; }

		[Since (6,0), MountainLion] // Yup, right, this is being "back-supported" to iOS 6
		[Export ("setRequiresSecureCoding:")]
		void SetRequiresSecureCoding (bool requireSecureEncoding);

		[Since (6,0), Mac(10,8)] // Yup, right, this is being back-supported to iOS 6
		[Export ("requiresSecureCoding")]
		bool GetRequiresSecureCoding ();
	}
	
	[BaseType (typeof (NSCoder),
		   Delegates=new string [] {"WeakDelegate"},
		   Events=new Type [] { typeof (NSKeyedUnarchiverDelegate) })]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSKeyedUnarchiver init]: cannot use -init for initialization
	[DisableDefaultCtor]
	public interface NSKeyedUnarchiver {
		[Export ("initForReadingWithData:")]
		[MarshalNativeExceptions]
		IntPtr Constructor (NSData data);
	
		[Static, Export ("unarchiveObjectWithData:")]
		[MarshalNativeExceptions]
		NSObject UnarchiveObject (NSData data);

		[Static, Export ("unarchiveTopLevelObjectWithData:error:")]
		[iOS (9,0), Mac(10,11)]
		// FIXME: [MarshalNativeExceptions]
		NSObject UnarchiveTopLevelObject (NSData data, out NSError error);
		
		[Static, Export ("unarchiveObjectWithFile:")]
		[MarshalNativeExceptions]
		NSObject UnarchiveFile (string file);

		[Export ("finishDecoding")]
		void FinishDecoding ();

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSKeyedUnarchiverDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClass:forClassName:")]
		void SetClass (Class kls, string codedName);

		[Export ("classForClassName:")]
		Class GetClass (string codedName);

		[Since (6,0), MountainLion] // Yup, right, this is being "back-supported" to iOS 6
		[Export ("setRequiresSecureCoding:")]
		void SetRequiresSecureCoding (bool requireSecureEncoding);

		[Since (6,0), Mac(10,8)] // Yup, right, this is being back-supported to iOS 6
		[Export ("requiresSecureCoding")]
		bool GetRequiresSecureCoding ();

	}

	[Since (5,0)]
	[BaseType (typeof (NSObject), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSMetadataQueryDelegate)})]
	public interface NSMetadataQuery {
		[Export ("startQuery")]
		bool StartQuery ();

		[Export ("stopQuery")]
		void StopQuery ();

		[Export ("isStarted")]
		bool IsStarted { get; }

		[Export ("isGathering")]
		bool IsGathering { get; }

		[Export ("isStopped")]
		bool IsStopped { get; }

		[Export ("disableUpdates")]
		void DisableUpdates ();

		[Export ("enableUpdates")]
		void EnableUpdates ();

		[Export ("resultCount")]
		nint ResultCount { get; }

		[Export ("resultAtIndex:")]
		NSObject ResultAtIndex (nint idx);

		[Export ("results")]
		NSMetadataItem[] Results { get; }

		[Export ("indexOfResult:")]
		nint IndexOfResult (NSObject result);

		[Export ("valueLists")]
		NSDictionary ValueLists { get; }

		[Export ("groupedResults")]
		NSObject [] GroupedResults { get; }

		[Export ("valueOfAttribute:forResultAtIndex:")]
		NSObject ValueOfAttribute (string attribyteName, nint atIndex);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSMetadataQueryDelegate Delegate { get; set; }
		
		[Export ("predicate", ArgumentSemantic.Copy)]
		[NullAllowed] // by default this property is null
		NSPredicate Predicate { get; set; }

		[Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor[] SortDescriptors { get; set; }

		[Export ("valueListAttributes", ArgumentSemantic.Copy)]
		NSObject[] ValueListAttributes { get; set; }

		[Export ("groupingAttributes", ArgumentSemantic.Copy)]
		NSArray GroupingAttributes { get; set; }

		[Export ("notificationBatchingInterval")]
		double NotificationBatchingInterval { get; set; }

		[Export ("searchScopes", ArgumentSemantic.Copy)]
		NSObject [] SearchScopes { get; set; }
		
		// There is no info associated with these notifications
		[Field ("NSMetadataQueryDidStartGatheringNotification")]
		[Notification]
		NSString DidStartGatheringNotification { get; }
	
		[Field ("NSMetadataQueryGatheringProgressNotification")]
		[Notification]
		NSString GatheringProgressNotification { get; }
		
		[Field ("NSMetadataQueryDidFinishGatheringNotification")]
		[Notification]
		NSString DidFinishGatheringNotification { get; }
		
		[Field ("NSMetadataQueryDidUpdateNotification")]
		[Notification]
		NSString DidUpdateNotification { get; }
		
		[Field ("NSMetadataQueryResultContentRelevanceAttribute")]
		NSString ResultContentRelevanceAttribute { get; }
		
		// Scope constants for defined search locations
#if MONOMAC
		[Field ("NSMetadataQueryUserHomeScope")]
		NSString UserHomeScope { get; }
		
		[Field ("NSMetadataQueryLocalComputerScope")]
		NSString LocalComputerScope { get; }

#if !XAMCORE_2_0
		[Field ("NSMetadataQueryNetworkScope")]
		[Obsolete ("Use NetworkScope")]
		NSString QueryNetworkScope { get; }

		[Field ("NSMetadataQueryLocalDocumentsScope")]
		[Obsolete ("Use LocalDocumentsScope")]
		NSString QueryLocalDocumentsScope { get; }

#endif 
		[Field ("NSMetadataQueryLocalDocumentsScope")]
		NSString LocalDocumentsScope { get; }

		[Field ("NSMetadataQueryNetworkScope")]
		NSString NetworkScope { get; }

#endif

#if !XAMCORE_2_0
		[Field ("NSMetadataQueryUbiquitousDocumentsScope")]
		[Obsolete ("Use UbiquitousDocumentsScope instead")]
		NSString QueryUbiquitousDocumentsScope { get; }

		[Field ("NSMetadataQueryUbiquitousDataScope")]
		[Obsolete ("Use UbiquitousDataScope instead")]
		NSString QueryUbiquitousDataScope { get; }
#endif

		[Field ("NSMetadataQueryUbiquitousDocumentsScope")]
		NSString UbiquitousDocumentsScope { get; }

		[Field ("NSMetadataQueryUbiquitousDataScope")]
		NSString UbiquitousDataScope { get; }


		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataQueryAccessibleUbiquitousExternalDocumentsScope")]
		NSString AccessibleUbiquitousExternalDocumentsScope { get; }

		[Field ("NSMetadataItemFSNameKey")]
		NSString ItemFSNameKey { get; }

		[Field ("NSMetadataItemDisplayNameKey")]
		NSString ItemDisplayNameKey { get; }

		[Field ("NSMetadataItemURLKey")]
		NSString ItemURLKey { get; }

		[Field ("NSMetadataItemPathKey")]
		NSString ItemPathKey { get; }

		[Field ("NSMetadataItemFSSizeKey")]
		NSString ItemFSSizeKey { get; }

		[Field ("NSMetadataItemFSCreationDateKey")]
		NSString ItemFSCreationDateKey { get; }

		[Field ("NSMetadataItemFSContentChangeDateKey")]
		NSString ItemFSContentChangeDateKey { get; }

		[iOS(8,0),Mac(10,9)]
		[Field ("NSMetadataItemContentTypeKey")]
		NSString ContentTypeKey { get; }

		[iOS(8,0),Mac(10,9)]
		[Field ("NSMetadataItemContentTypeTreeKey")]
		NSString ContentTypeTreeKey { get; }
		

		[Field ("NSMetadataItemIsUbiquitousKey")]
		NSString ItemIsUbiquitousKey { get; }

		[Field ("NSMetadataUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		[Field ("NSMetadataUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		[Field ("NSMetadataUbiquitousItemIsDownloadingKey")]
		NSString UbiquitousItemIsDownloadingKey { get; }

		[Field ("NSMetadataUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		[Field ("NSMetadataUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Field ("NSMetadataUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Field ("NSMetadataUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Field ("NSMetadataUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		[Field ("NSMetadataUbiquitousItemPercentDownloadedKey")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		[Field ("NSMetadataUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemDownloadRequestedKey")]
		NSString UbiquitousItemDownloadRequestedKey { get; }

		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemIsExternalDocumentKey")]
		NSString UbiquitousItemIsExternalDocumentKey { get; }
		
		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }
		
		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemURLInLocalContainerKey")]
		NSString UbiquitousItemURLInLocalContainerKey { get; }
		
		[Since (7,0), Mavericks]
		[NullAllowed] // by default this property is null
		[Export ("searchItems", ArgumentSemantic.Copy)]
		// DOC: object is a mixture of NSString, NSMetadataItem, NSUrl
		NSObject [] SearchItems { get; set; }

		[Since (7,0), Mavericks]
		[NullAllowed] // by default this property is null
		[Export ("operationQueue", ArgumentSemantic.Retain)]
		NSOperationQueue OperationQueue { get; set; }
		
		[Since (7,0), Mavericks]
		[Export ("enumerateResultsUsingBlock:")]
		void EnumerateResultsUsingBlock (NSMetadataQueryEnumerationCallback callback);

		[Since (7,0), Mavericks, Export ("enumerateResultsWithOptions:usingBlock:")]
		void EnumerateResultsWithOptions (NSEnumerationOptions opts, NSMetadataQueryEnumerationCallback block);

		//
		// These are for NSMetadataQueryDidUpdateNotification 
		//
		[Mac (10,9)][iOS (8,0)]
		[Field ("NSMetadataQueryUpdateAddedItemsKey")]
		NSString QueryUpdateAddedItemsKey { get; }

		[Mac (10,9)][iOS (8,0)]
		[Field ("NSMetadataQueryUpdateChangedItemsKey")]
		NSString QueryUpdateChangedItemsKey { get; }
		
		[Mac (10,9)][iOS (8,0)]
		[Field ("NSMetadataQueryUpdateRemovedItemsKey")]
		NSString QueryUpdateRemovedItemsKey { get; }
	}

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSMetadataQueryDelegate {
		[Export ("metadataQuery:replacementObjectForResultObject:"), DelegateName ("NSMetadataQueryObject"), DefaultValue(null)]
		NSObject ReplacementObjectForResultObject (NSMetadataQuery query, NSMetadataItem result);

		[Export ("metadataQuery:replacementValueForAttribute:value:"), DelegateName ("NSMetadataQueryValue"), DefaultValue(null)]
		NSObject ReplacementValueForAttributevalue (NSMetadataQuery query, string attributeName, NSObject value);
	}

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	public interface NSMetadataItem {
		[Export ("valueForAttribute:")]
		NSObject ValueForAttribute (string key);

		[Export ("valuesForAttributes:")]
		NSDictionary ValuesForAttributes (NSArray keys);

		[Export ("attributes")]
		NSObject [] Attributes { get; }

		[Mac(10,9),iOS(7,0)]
		[Internal]
		[Field ("NSMetadataUbiquitousItemDownloadingStatusCurrent")]
		NSString _StatusCurrent { get; }

		[Mac(10,9),iOS(7,0)]
		[Internal]
		[Field ("NSMetadataUbiquitousItemDownloadingStatusDownloaded")]
		NSString _StatusDownloaded { get; }

		[Mac(10,9),iOS(7,0)]
		[Internal]
		[Field ("NSMetadataUbiquitousItemDownloadingStatusNotDownloaded")]
		NSString _NotDownloaded { get; }
		
	}

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	public interface NSMetadataQueryAttributeValueTuple {
		[Export ("attribute")]
		string Attribute { get; }

		[Export ("value")]
		NSObject Value { get; }

		[Export ("count")]
		nint Count { get; }
	}

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	public interface NSMetadataQueryResultGroup {
		[Export ("attribute")]
		string Attribute { get; }

		[Export ("value")]
		NSObject Value { get; }

		[Export ("subgroups")]
		NSObject [] Subgroups { get; }

		[Export ("resultCount")]
		nint ResultCount { get; }

		[Export ("resultAtIndex:")]
		NSObject ResultAtIndex (nuint idx);

		[Export ("results")]
		NSObject [] Results { get; }

	}

	// Sadly, while this API is a poor API and we should in general not use it
	// Apple has now surfaced it on a few methods.   So we need to take the Obsolete
	// out, and we will have to fully support it.
	[BaseType (typeof (NSArray))]
	public interface NSMutableArray {
		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		IntPtr Constructor (nuint capacity);

		[Internal]
		[Sealed]
		[Export ("addObject:")]
		void _Add (IntPtr obj);

		[Export ("addObject:")]
		void Add (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("insertObject:atIndex:")]
		void _Insert (IntPtr obj, nint index);

		[Export ("insertObject:atIndex:")]
		void Insert (NSObject obj, nint index);

		[Export ("removeLastObject")]
		void RemoveLastObject ();

		[Export ("removeObjectAtIndex:")]
		void RemoveObject (nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectAtIndex:withObject:")]
		void _ReplaceObject (nint index, IntPtr withObject);

		[Export ("replaceObjectAtIndex:withObject:")]
		void ReplaceObject (nint index, NSObject withObject);

		[Export ("removeAllObjects")]
		void RemoveAllObjects ();

		[Export ("addObjectsFromArray:")]
		void AddObjects (NSObject [] source);

		[Internal]
		[Sealed]
		[Export ("insertObjects:atIndexes:")]
		void _InsertObjects (IntPtr objects, NSIndexSet atIndexes);

		[Export ("insertObjects:atIndexes:")]
		void InsertObjects (NSObject [] objects, NSIndexSet atIndexes);

		[Export ("removeObjectsAtIndexes:")]
		void RemoveObjectsAtIndexes (NSIndexSet indexSet);

		[iOS (8,0), Mac(10,10)]
		[Static, Export ("arrayWithContentsOfFile:")]
		NSMutableArray FromFile (string path);

		[iOS (8,0), Mac(10,10)]
		[Static, Export ("arrayWithContentsOfURL:")]
		NSMutableArray FromUrl (NSUrl url);
		
	}
	
	public interface NSMutableArray<TValue> : NSMutableArray {}

	[Since (3,2)]
	[BaseType (typeof (NSAttributedString))]
	public interface NSMutableAttributedString {
		[Export ("initWithString:")]
		IntPtr Constructor (string str);
		
		[Export ("initWithString:attributes:")]
		IntPtr Constructor (string str, [NullAllowed] NSDictionary attributes);

		[Export ("initWithAttributedString:")]
		IntPtr Constructor (NSAttributedString other);

		[Export ("replaceCharactersInRange:withString:")]
		void Replace (NSRange range, string newValue);

		[Export ("setAttributes:range:")]
		void LowLevelSetAttributes (IntPtr dictionaryAttrsHandle, NSRange range);

		[Export ("mutableString", ArgumentSemantic.Retain)]
		NSMutableString MutableString { get; }

		[Export ("addAttribute:value:range:")]
		void AddAttribute (NSString attributeName, NSObject value, NSRange range);

		[Export ("addAttributes:range:")]
		void AddAttributes (NSDictionary attrs, NSRange range);

#if MONOMAC
		[Wrap ("AddAttributes (attributes == null ? null : attributes.Dictionary, range)")]
		void AddAttributes (NSStringAttributes attributes, NSRange range);
#endif
		[Export ("removeAttribute:range:")]
		void RemoveAttribute (string name, NSRange range);
		
		[Export ("replaceCharactersInRange:withAttributedString:")]
		void Replace (NSRange range, NSAttributedString value);
		
		[Export ("insertAttributedString:atIndex:")]
		void Insert (NSAttributedString attrString, nint location);

		[Export ("appendAttributedString:")]
		void Append (NSAttributedString attrString);

		[Export ("deleteCharactersInRange:")]
		void DeleteRange (NSRange range);

		[Export ("setAttributedString:")]
		void SetString (NSAttributedString attrString);

		[Export ("beginEditing")]
		void BeginEditing ();

		[Export ("endEditing")]
		void EndEditing ();

#if !MONOMAC
		[NoTV]
		[Since (7,0)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use ReadFromUrl")]
		[Export ("readFromFileURL:options:documentAttributes:error:")]
		bool ReadFromFile (NSUrl url, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use ReadFromUrl")]
		[Wrap ("ReadFromFile (url, options == null ? null : options.Dictionary, ref returnOptions, ref error)")]
		bool ReadFromFile (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

		[Since (7,0)]
		[Export ("readFromData:options:documentAttributes:error:")]
		bool ReadFromData (NSData data, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);
		
		[Wrap ("ReadFromData (data, options == null ? null : options.Dictionary, ref returnOptions, ref error)")]
		bool ReadFromData (NSData data, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

#endif

#if XAMCORE_2_0
		[Internal]
		[Sealed]
		[iOS(9,0), Mac(10,11)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);
#endif

		[iOS(9,0), Mac(10,11)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary<NSString, NSObject> options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);

		[iOS(9,0), Mac(10,11)]
		[Wrap ("ReadFromUrl (url, options.Dictionary, ref returnOptions, ref error)")]
		bool ReadFromUrl (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);
	}

	[BaseType (typeof (NSData))]
	public interface NSMutableData {
		[Static, Export ("dataWithCapacity:")] [Autorelease]
		[PreSnippet ("if (capacity < 0 || capacity > nint.MaxValue) throw new ArgumentOutOfRangeException ();")]
		NSMutableData FromCapacity (nint capacity);

		[Static, Export ("dataWithLength:")] [Autorelease]
		[PreSnippet ("if (length < 0 || length > nint.MaxValue) throw new ArgumentOutOfRangeException ();")]
		NSMutableData FromLength (nint length);
		
		[Static, Export ("data")] [Autorelease]
		NSMutableData Create ();

		[Export ("mutableBytes")]
		IntPtr MutableBytes { get; }

		[Export ("initWithCapacity:")]
		[PreSnippet ("if (capacity > (ulong) nint.MaxValue) throw new ArgumentOutOfRangeException ();")]
		IntPtr Constructor (nuint capacity);

		[Export ("appendData:")]
		void AppendData (NSData other);

		[Export ("appendBytes:length:")]
		void AppendBytes (IntPtr bytes, nuint len);

		[Export ("setData:")]
		void SetData (NSData data);

		[Export ("length")]
		[Override]
		nuint Length { get; set; }

		[Export ("replaceBytesInRange:withBytes:")]
		void ReplaceBytes (NSRange range, IntPtr buffer);

		[Export ("resetBytesInRange:")]
		void ResetBytes (NSRange range);

		[Export ("replaceBytesInRange:withBytes:length:")]
		void ReplaceBytes (NSRange range, IntPtr buffer, nuint length);
		
	}

	[BaseType (typeof (NSObject))]
	public interface NSDate : NSSecureCoding, NSCopying {
		[Export ("timeIntervalSinceReferenceDate")]
		double SecondsSinceReferenceDate { get; }

		[Export ("dateWithTimeIntervalSinceReferenceDate:")]
		[Static]
		NSDate FromTimeIntervalSinceReferenceDate (double secs);

		[Static, Export ("dateWithTimeIntervalSince1970:")]
		NSDate FromTimeIntervalSince1970 (double secs);

		[Export ("date")]
		[Static]
		NSDate Now { get; }
		
		[Export ("distantPast")]
		[Static]
		NSDate DistantPast { get; }
		
		[Export ("distantFuture")]
		[Static]
		NSDate DistantFuture { get; }

		[Export ("dateByAddingTimeInterval:")]
		NSDate AddSeconds (double seconds);

		[Export ("dateWithTimeIntervalSinceNow:")]
		[Static]
		NSDate FromTimeIntervalSinceNow (double secs);

		[Export ("descriptionWithLocale:")]
		string DescriptionWithLocale (NSLocale locale);

		[Export ("earlierDate:")]
		NSDate EarlierDate (NSDate anotherDate);

		[Export ("laterDate:")]
		NSDate LaterDate (NSDate anotherDate);

		[Export ("compare:")]
		NSComparisonResult Compare (NSDate other);

		[Export ("isEqualToDate:")]
		bool IsEqualToDate (NSDate other);
	}
	
	[BaseType (typeof (NSObject))]
	public interface NSDictionary : NSSecureCoding, NSMutableCopying {
		[Export ("dictionaryWithContentsOfFile:")]
		[Static]
		NSDictionary FromFile (string path);

		[Export ("dictionaryWithContentsOfURL:")]
		[Static]
		NSDictionary FromUrl (NSUrl url);

		[Export ("dictionaryWithObject:forKey:")]
		[Static]
		NSDictionary FromObjectAndKey (NSObject obj, NSObject key);

		[Export ("dictionaryWithDictionary:")]
		[Static]
		NSDictionary FromDictionary (NSDictionary source);

		[Export ("dictionaryWithObjects:forKeys:count:")]
		[Static, Internal]
		IntPtr _FromObjectsAndKeysInternal (IntPtr objects, IntPtr keys, nint count);

		[Export ("dictionaryWithObjects:forKeys:count:")]
		[Static, Internal]
		NSDictionary FromObjectsAndKeysInternal ([NullAllowed] NSArray objects, [NullAllowed] NSArray keys, nint count);

		[Export ("dictionaryWithObjects:forKeys:")]
		[Static, Internal]
		IntPtr _FromObjectsAndKeysInternal (IntPtr objects, IntPtr keys);

		[Export ("dictionaryWithObjects:forKeys:")]
		[Static, Internal]
		NSDictionary FromObjectsAndKeysInternal ([NullAllowed] NSArray objects, [NullAllowed] NSArray keys);

		[Export ("initWithDictionary:")]
		IntPtr Constructor (NSDictionary other);

		[Export ("initWithContentsOfFile:")]
		IntPtr Constructor (string fileName);

		[Export ("initWithObjects:forKeys:"), Internal]
		IntPtr Constructor (NSArray objects, NSArray keys);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);
		
		[Export ("count")]
		nuint Count { get; }

		[Internal]
		[Sealed]
		[Export ("objectForKey:")]
		IntPtr _ObjectForKey (IntPtr key);

		[Export ("objectForKey:")]
		NSObject ObjectForKey (NSObject key);

		[Internal]
		[Sealed]
		[Export ("allKeys")]
		IntPtr _AllKeys ();

		[Export ("allKeys")][Autorelease]
		NSObject [] Keys { get; }

		[Internal]
		[Sealed]
		[Export ("allKeysForObject:")]
		IntPtr _AllKeysForObject (IntPtr obj);

		[Export ("allKeysForObject:")][Autorelease]
		NSObject [] KeysForObject (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("allValues")]
		IntPtr _AllValues ();

		[Export ("allValues")][Autorelease]
		NSObject [] Values { get; }

		[Export ("descriptionInStringsFileFormat")]
		string DescriptionInStringsFileFormat { get; }

		[Export ("isEqualToDictionary:")]
		bool IsEqualToDictionary (NSDictionary other);
		
		[Export ("objectEnumerator")]
		NSEnumerator ObjectEnumerator { get; }

		[Internal]
		[Sealed]
		[Export ("objectsForKeys:notFoundMarker:")]
		IntPtr _ObjectsForKeys (IntPtr keys, IntPtr marker);

		[Export ("objectsForKeys:notFoundMarker:")][Autorelease]
		NSObject [] ObjectsForKeys (NSArray keys, NSObject marker);
		
		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Export ("writeToURL:atomically:")]
		bool WriteToUrl (NSUrl url, bool atomically);

		[Since (6,0)]
		[Static]
		[Export ("sharedKeySetForKeys:")]
		NSObject GetSharedKeySetForKeys (NSObject [] keys);

	}

	public interface NSDictionary<K,V> : NSDictionary {}

	[BaseType (typeof (NSObject))]
	public interface NSEnumerator {
		[Export ("nextObject")]
		NSObject NextObject (); 
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface NSError : NSSecureCoding, NSCopying {
		[Static, Export ("errorWithDomain:code:userInfo:")]
		NSError FromDomain (NSString domain, nint code, [NullAllowed] NSDictionary userInfo);

		[DesignatedInitializer]
		[Export ("initWithDomain:code:userInfo:")]
		IntPtr Constructor (NSString domain, nint code, [NullAllowed] NSDictionary userInfo);
		
		[Export ("domain")]
		string Domain { get; }

		[Export ("code")]
		nint Code { get; }

		[Export ("userInfo")]
		NSDictionary UserInfo { get; }

		[Export ("localizedDescription")]
		string LocalizedDescription { get; }

		[Export ("localizedFailureReason")]
		string LocalizedFailureReason { get; }

		[Export ("localizedRecoverySuggestion")]
		string LocalizedRecoverySuggestion { get; }

		[Export ("localizedRecoveryOptions")]
		string [] LocalizedRecoveryOptions { get; }

		[Export ("helpAnchor")]
		string HelpAnchor { get; }

		[Field ("NSCocoaErrorDomain")]
		NSString CocoaErrorDomain { get;}

		[Field ("NSPOSIXErrorDomain")]
		NSString PosixErrorDomain { get; }

		[Field ("NSOSStatusErrorDomain")]
		NSString OsStatusErrorDomain { get; }

		[Field ("NSMachErrorDomain")]
		NSString MachErrorDomain { get; }

		[Field ("NSURLErrorDomain")]
		NSString NSUrlErrorDomain { get; }

		[Field ("NSNetServicesErrorDomain")]
		NSString NSNetServicesErrorDomain { get; }

		[Field ("NSStreamSocketSSLErrorDomain")]
		NSString NSStreamSocketSSLErrorDomain { get; }

		[Field ("NSStreamSOCKSErrorDomain")]
		NSString NSStreamSOCKSErrorDomain { get; }

		[Field ("kCLErrorDomain", "CoreLocation")]
		NSString CoreLocationErrorDomain { get; }

#if !WATCH
		[Field ("kCFErrorDomainCFNetwork", "CFNetwork")]
		NSString CFNetworkErrorDomain { get; }
#endif

		[NoMac, NoTV]
		[Field ("CMErrorDomain", "CoreMotion")]
		NSString CoreMotionErrorDomain { get; }

#if !XAMCORE_3_0
		// now exposed with the corresponding EABluetoothAccessoryPickerError enum
		[NoMac, NoTV, NoWatch]
		[iOS (6,0)]
		[NoTV]
		[Field ("EABluetoothAccessoryPickerErrorDomain", "ExternalAccessory")]
		NSString EABluetoothAccessoryPickerErrorDomain { get; }

		// now exposed with the corresponding MKErrorCode enum
		[TV (9,2)]
		[NoMac][NoWatch]
		[Field ("MKErrorDomain", "MapKit")]
		NSString MapKitErrorDomain { get; }

		// now exposed with the corresponding WKErrorCode enum
		[NoMac, NoTV]
		[iOS (8,2)]
		[Field ("WatchKitErrorDomain", "WatchKit")]
		NSString WatchKitErrorDomain { get; }
#endif
		
		[Field ("NSUnderlyingErrorKey")]
		NSString UnderlyingErrorKey { get; }

		[Field ("NSLocalizedDescriptionKey")]
		NSString LocalizedDescriptionKey { get; }

		[Field ("NSLocalizedFailureReasonErrorKey")]
		NSString LocalizedFailureReasonErrorKey { get; }

		[Field ("NSLocalizedRecoverySuggestionErrorKey")]
		NSString LocalizedRecoverySuggestionErrorKey { get; }

		[Field ("NSLocalizedRecoveryOptionsErrorKey")]
		NSString LocalizedRecoveryOptionsErrorKey { get; }

		[Field ("NSRecoveryAttempterErrorKey")]
		NSString RecoveryAttempterErrorKey { get; }

		[Field ("NSHelpAnchorErrorKey")]
		NSString HelpAnchorErrorKey { get; }

		[Field ("NSStringEncodingErrorKey")]
		NSString StringEncodingErrorKey { get; }

		[Field ("NSURLErrorKey")]
		NSString UrlErrorKey { get; }

		[Field ("NSFilePathErrorKey")]
		NSString FilePathErrorKey { get; }

		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("setUserInfoValueProviderForDomain:provider:")]
		void SetUserInfoValueProvider (string errorDomain, [NullAllowed] NSErrorUserInfoValueProvider provider);

		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("userInfoValueProviderForDomain:")]
		[return: NullAllowed]
		NSErrorUserInfoValueProvider GetUserInfoValueProvider (string errorDomain);
		
#if false
		// FIXME that value is present in the header (7.0 DP 6) files but returns NULL (i.e. unusable)
		// we're also missing other NSURLError* fields (which we should add)
		[Since (7,0)]
		[Field ("NSURLErrorBackgroundTaskCancelledReasonKey")]
		NSString NSUrlErrorBackgroundTaskCancelledReasonKey { get; }
#endif
	}

	public delegate NSObject NSErrorUserInfoValueProvider (NSError error, NSString userInfoKey);	

	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	public interface NSException : NSCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithName:reason:userInfo:")]
		IntPtr Constructor (string name, string reason, [NullAllowed] NSDictionary userInfo);

		[Export ("name")]
		string Name { get; }
	
		[Export ("reason")]
		string Reason { get; }
		
		[Export ("userInfo")]
		NSObject UserInfo { get; }

		[Export ("callStackReturnAddresses")]
		NSNumber[] CallStackReturnAddresses { get; }

		[Export ("callStackSymbols")]
		string[] CallStackSymbols { get; }
	}

#if !XAMCORE_4_0
	[Obsolete("NSExpressionHandler is deprecated, please use FromFormat (string, NSObject[]) instead.")]
	public delegate void NSExpressionHandler (NSObject evaluatedObject, NSExpression [] expressions, NSMutableDictionary context);
#endif
	public delegate NSObject NSExpressionCallbackHandler (NSObject evaluatedObject, NSExpression [] expressions, NSMutableDictionary context);
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -predicateFormat cannot be sent to an abstract object of class NSExpression: Create a concrete instance!
	[DisableDefaultCtor]
	public interface NSExpression : NSSecureCoding, NSCopying {
		[Static, Export ("expressionForConstantValue:")]
		NSExpression FromConstant (NSObject obj);

		[Static, Export ("expressionForEvaluatedObject")]
		NSExpression ExpressionForEvaluatedObject { get; }

		[Static, Export ("expressionForVariable:")]
		NSExpression FromVariable (string string1);

		[Static, Export ("expressionForKeyPath:")]
		NSExpression FromKeyPath (string keyPath);

		[Static, Export ("expressionForFunction:arguments:")]
		NSExpression FromFunction (string name, NSExpression[] parameters);

		[Static, Export ("expressionWithFormat:")]
		NSExpression FromFormat (string expressionFormat);

#if !XAMCORE_4_0
		[Obsolete("FromFormat (string, NSExpression[]) is deprecated, please use FromFormat (string, NSObject[]) instead.")]
		[Static, Export ("expressionWithFormat:argumentArray:")]
		NSExpression FromFormat (string format, NSExpression [] parameters);
#endif

		[Static, Export ("expressionWithFormat:argumentArray:")]
		NSExpression FromFormat (string format, NSObject [] parameters);

		//+ (NSExpression *)expressionForAggregate:(NSArray *)subexpressions; 
		[Static, Export ("expressionForAggregate:")]
		NSExpression FromAggregate (NSExpression [] subexpressions);

		[Static, Export ("expressionForUnionSet:with:")]
		NSExpression FromUnionSet (NSExpression left, NSExpression right);

		[Static, Export ("expressionForIntersectSet:with:")]
		NSExpression FromIntersectSet (NSExpression left, NSExpression right);

		[Static, Export ("expressionForMinusSet:with:")]
		NSExpression FromMinusSet (NSExpression left, NSExpression right);

		//+ (NSExpression *)expressionForSubquery:(NSExpression *)expression usingIteratorVariable:(NSString *)variable predicate:(id)predicate; 
		[Static, Export ("expressionForSubquery:usingIteratorVariable:predicate:")]
		NSExpression FromSubquery (NSExpression expression, string variable, NSObject predicate);

		[Static, Export ("expressionForFunction:selectorName:arguments:")]
		NSExpression FromFunction (NSExpression target, string name, NSExpression[] parameters);

#if !XAMCORE_4_0
		[Obsolete("FromFunction (NSExpressionHandler, NSExpression[]) is deprecated, please use FromFunction (NSExpressionCallbackHandler, NSExpression[]) instead.")]
		[Static, Export ("expressionForBlock:arguments:")]
		NSExpression FromFunction (NSExpressionHandler target, NSExpression[] parameters);
#endif

		[Static, Export ("expressionForBlock:arguments:")]
		NSExpression FromFunction (NSExpressionCallbackHandler target, NSExpression[] parameters);

		[Since (7,0), Mavericks]
		[Static]
		[Export ("expressionForAnyKey")]
		NSExpression FromAnyKey ();

		[iOS(9,0),Mac(10,11)]
		[Static]
		[Export ("expressionForConditional:trueExpression:falseExpression:")]
		NSExpression FromConditional (NSPredicate predicate, NSExpression trueExpression, NSExpression falseExpression);
			
		[Since (7,0), Mavericks]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();
		
		[DesignatedInitializer]
		[Export ("initWithExpressionType:")]
		IntPtr Constructor (NSExpressionType type);

		[Export ("expressionType")]
		NSExpressionType ExpressionType { get; }

		[Sealed, Internal, Export ("expressionBlock")]
		NSExpressionCallbackHandler _Block { get; }

		[Sealed, Internal, Export ("constantValue")]
		NSObject _ConstantValue { get; }

		[Sealed, Internal, Export ("keyPath")]
		string _KeyPath { get; }

		[Sealed, Internal, Export ("function")]
		string _Function { get; }

		[Sealed, Internal, Export ("variable")]
		string _Variable { get; }

		[Sealed, Internal, Export ("operand")]
		NSExpression _Operand { get; }

		[Sealed, Internal, Export ("arguments")]
		NSExpression[] _Arguments { get; }

		[Sealed, Internal, Export ("collection")]
		NSObject _Collection { get; }

		[Sealed, Internal, Export ("predicate")]
		NSPredicate _Predicate { get; }

		[Sealed, Internal, Export ("leftExpression")]
		NSExpression _LeftExpression { get; }

		[Sealed, Internal, Export ("rightExpression")]
		NSExpression _RightExpression { get; }

		[Mac(10,11),iOS(9,0)]
		[Sealed, Internal, Export ("trueExpression")]
		NSExpression _TrueExpression { get; }

		[Mac(10,11),iOS(9,0)]
		[Sealed, Internal, Export ("falseExpression")]
		NSExpression _FalseExpression { get; }
		
		[Export ("expressionValueWithObject:context:")]
		NSObject EvaluateWith ([NullAllowed] NSObject obj, [NullAllowed] NSMutableDictionary context);
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	public partial interface NSExtensionContext {

		[Export ("inputItems", ArgumentSemantic.Copy)]
		NSExtensionItem [] InputItems { get; }

		[Export ("completeRequestReturningItems:completionHandler:")]
		void CompleteRequest (NSExtensionItem [] returningItems, [NullAllowed] Action<bool> completionHandler);

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);

		[Export ("openURL:completionHandler:")]
		[Async]
		void OpenUrl (NSUrl url, [NullAllowed] Action<bool> completionHandler);

		[Field ("NSExtensionItemsAndErrorsKey")]
		NSString ItemsAndErrorsKey { get; }

#if !MONOMAC
		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostWillEnterForegroundNotification")]
		NSString HostWillEnterForegroundNotification { get; }

		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostDidEnterBackgroundNotification")]
		NSString HostDidEnterBackgroundNotification { get; }

		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostWillResignActiveNotification")]
		NSString HostWillResignActiveNotification { get; }

		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostDidBecomeActiveNotification")]
		NSString HostDidBecomeActiveNotification { get; }
#endif
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	public partial interface NSExtensionItem : NSCopying, NSSecureCoding {

		[NullAllowed] // by default this property is null
		[Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("attributedContentText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedContentText { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("attachments", ArgumentSemantic.Copy)]
		NSItemProvider [] Attachments { get; set; }

		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; set; }

		[Field ("NSExtensionItemAttributedTitleKey")]
		NSString AttributedTitleKey { get; }

		[Field ("NSExtensionItemAttributedContentTextKey")]
		NSString AttributedContentTextKey { get; }

		[Field ("NSExtensionItemAttachmentsKey")]
		NSString AttachmentsKey { get; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSNull : NSSecureCoding, NSCopying {
		[Export ("null"), Static]
		NSNull Null { get; }
	}

	[iOS (8,0)]
	[Mac(10,10)]
	[BaseType (typeof (NSFormatter))]
	interface NSLengthFormatter {
		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("unitStyle")]
		NSFormattingUnitStyle UnitStyle { get; set; }

		[Export ("stringFromValue:unit:")]
		string StringFromValue (double value, NSLengthFormatterUnit unit);

		[Export ("stringFromMeters:")]
		string StringFromMeters (double numberInMeters);

		[Export ("unitStringFromValue:unit:")]
		string UnitStringFromValue (double value, NSLengthFormatterUnit unit);

		[Export ("unitStringFromMeters:usedUnit:")]
		string UnitStringFromMeters (double numberInMeters, ref NSLengthFormatterUnit unitp);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);

		[Export ("forPersonHeightUse")]
		bool ForPersonHeightUse { [Bind ("isForPersonHeightUse")] get; set; }
	}

	delegate void NSLingusticEnumerator (NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop);

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	interface NSLinguisticTagger {
		[DesignatedInitializer]
		[Export ("initWithTagSchemes:options:")]
		IntPtr Constructor (NSString [] tagSchemes, NSLinguisticTaggerOptions opts);

		[Export ("tagSchemes")]
		NSString [] TagSchemes { get; }

		[Static]
		[Export ("availableTagSchemesForLanguage:")]
		NSString [] GetAvailableTagSchemesForLanguage (string language);

		[Export ("setOrthography:range:")]
		void SetOrthographyrange (NSOrthography orthography, NSRange range);

		[Export ("orthographyAtIndex:effectiveRange:")]
		NSOrthography GetOrthography (nint charIndex, ref NSRange effectiveRange);

		[Export ("stringEditedInRange:changeInLength:")]
		void StringEditedInRange (NSRange newRange, nint delta);

		[Export ("enumerateTagsInRange:scheme:options:usingBlock:")]
		void EnumerateTagsInRange (NSRange range, NSString tagScheme, NSLinguisticTaggerOptions opts, NSLingusticEnumerator enumerator);

		[Export ("sentenceRangeForRange:")]
		NSRange GetSentenceRangeForRange (NSRange range);

		[Export ("tagAtIndex:scheme:tokenRange:sentenceRange:")]
		string GetTag (nint charIndex, NSString tagScheme, ref NSRange tokenRange, ref NSRange sentenceRange);

		[Export ("tagsInRange:scheme:options:tokenRanges:"), Internal]
		NSString [] GetTagsInRange (NSRange range, NSString tagScheme, NSLinguisticTaggerOptions opts, ref NSArray tokenRanges);

		[Export ("possibleTagsAtIndex:scheme:tokenRange:sentenceRange:scores:"), Internal]
		NSString [] GetPossibleTags (nint charIndex, NSString tagScheme, ref NSRange tokenRange, ref NSRange sentenceRange, ref NSArray scores);

		//Detected properties
		[NullAllowed] // by default this property is null
		[Export ("string", ArgumentSemantic.Retain)]
		string AnalysisString { get; set; }
	}

	[Since (5,0)]
	[Static]
	public interface NSLinguisticTag {
		[Field ("NSLinguisticTagSchemeTokenType")]
		NSString SchemeTokenType { get; }

		[Field ("NSLinguisticTagSchemeLexicalClass")]
		NSString SchemeLexicalClass { get; }

		[Field ("NSLinguisticTagSchemeNameType")]
		NSString SchemeNameType { get; }

		[Field ("NSLinguisticTagSchemeNameTypeOrLexicalClass")]
		NSString SchemeNameTypeOrLexicalClass { get; }

		[Field ("NSLinguisticTagSchemeLemma")]
		NSString SchemeLemma { get; }

		[Field ("NSLinguisticTagSchemeLanguage")]
		NSString SchemeLanguage { get; }

		[Field ("NSLinguisticTagSchemeScript")]
		NSString SchemeScript { get; }

		[Field ("NSLinguisticTagWord")]
		NSString Word { get; }

		[Field ("NSLinguisticTagPunctuation")]
		NSString Punctuation { get; }

		[Field ("NSLinguisticTagWhitespace")]
		NSString Whitespace { get; }

		[Field ("NSLinguisticTagOther")]
		NSString Other { get; }

		[Field ("NSLinguisticTagNoun")]
		NSString Noun { get; }

		[Field ("NSLinguisticTagVerb")]
		NSString Verb { get; }

		[Field ("NSLinguisticTagAdjective")]
		NSString Adjective { get; }

		[Field ("NSLinguisticTagAdverb")]
		NSString Adverb { get; }

		[Field ("NSLinguisticTagPronoun")]
		NSString Pronoun { get; }

		[Field ("NSLinguisticTagDeterminer")]
		NSString Determiner { get; }

		[Field ("NSLinguisticTagParticle")]
		NSString Particle { get; }

		[Field ("NSLinguisticTagPreposition")]
		NSString Preposition { get; }

		[Field ("NSLinguisticTagNumber")]
		NSString Number { get; }

		[Field ("NSLinguisticTagConjunction")]
		NSString Conjunction { get; }

		[Field ("NSLinguisticTagInterjection")]
		NSString Interjection { get; }

		[Field ("NSLinguisticTagClassifier")]
		NSString Classifier { get; }

		[Field ("NSLinguisticTagIdiom")]
		NSString Idiom { get; }

		[Field ("NSLinguisticTagOtherWord")]
		NSString OtherWord { get; }

		[Field ("NSLinguisticTagSentenceTerminator")]
		NSString SentenceTerminator { get; }

		[Field ("NSLinguisticTagOpenQuote")]
		NSString OpenQuote { get; }

		[Field ("NSLinguisticTagCloseQuote")]
		NSString CloseQuote { get; }

		[Field ("NSLinguisticTagOpenParenthesis")]
		NSString OpenParenthesis { get; }

		[Field ("NSLinguisticTagCloseParenthesis")]
		NSString CloseParenthesis { get; }

		[Field ("NSLinguisticTagWordJoiner")]
		NSString WordJoiner { get; }

		[Field ("NSLinguisticTagDash")]
		NSString Dash { get; }

		[Field ("NSLinguisticTagOtherPunctuation")]
		NSString OtherPunctuation { get; }

		[Field ("NSLinguisticTagParagraphBreak")]
		NSString ParagraphBreak { get; }

		[Field ("NSLinguisticTagOtherWhitespace")]
		NSString OtherWhitespace { get; }

		[Field ("NSLinguisticTagPersonalName")]
		NSString PersonalName { get; }

		[Field ("NSLinguisticTagPlaceName")]
		NSString PlaceName { get; }

		[Field ("NSLinguisticTagOrganizationName")]
		NSString OrganizationName { get; }
	}
	
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL so it's not usable evenif it does not throw an ObjC exception
	// funnily it was "added" in iOS 7 and header files says "do not invoke; not a valid initializer for this class"
	[DisableDefaultCtor]
	public interface NSLocale : NSSecureCoding, NSCopying {
		[Static]
		[Export ("systemLocale")]
		NSLocale SystemLocale { get; }

		[Static]
		[Export ("currentLocale")]
		NSLocale CurrentLocale { get; }

		[Static]
		[Export ("autoupdatingCurrentLocale")]
		NSLocale AutoUpdatingCurrentLocale { get; }
		
		[DesignatedInitializer]
		[Export ("initWithLocaleIdentifier:")]
		IntPtr Constructor (string identifier);

		[Export ("localeIdentifier")]
		string LocaleIdentifier { get; }

		[Export ("availableLocaleIdentifiers")][Static]
		string [] AvailableLocaleIdentifiers { get; }

		[Export ("ISOLanguageCodes")][Static]
		string [] ISOLanguageCodes { get; }

		[Export ("ISOCurrencyCodes")][Static]
		string [] ISOCurrencyCodes { get; }

		[Export ("ISOCountryCodes")][Static]
		string [] ISOCountryCodes { get; }

		[Export ("commonISOCurrencyCodes")][Static]
		string [] CommonISOCurrencyCodes { get; }

		[Export ("preferredLanguages")][Static]
		string [] PreferredLanguages { get; }

		[Export ("componentsFromLocaleIdentifier:")][Static]
		NSDictionary ComponentsFromLocaleIdentifier (string identifier);

		[Export ("localeIdentifierFromComponents:")][Static]
		string LocaleIdentifierFromComponents (NSDictionary dict);

		[Export ("canonicalLanguageIdentifierFromString:")][Static]
		string CanonicalLanguageIdentifierFromString (string str);

		[Export ("canonicalLocaleIdentifierFromString:")][Static]
		string CanonicalLocaleIdentifierFromString (string str);

		[Export ("characterDirectionForLanguage:")][Static]
		NSLocaleLanguageDirection GetCharacterDirection (string isoLanguageCode);

		[Export ("lineDirectionForLanguage:")][Static]
		NSLocaleLanguageDirection GetLineDirection (string isoLanguageCode);

		[Since (7,0)] // already in OSX 10.6
		[Static]
		[Export ("localeWithLocaleIdentifier:")]
		NSLocale FromLocaleIdentifier (string ident);

		[Field ("NSCurrentLocaleDidChangeNotification")]
		[Notification]
		NSString CurrentLocaleDidChangeNotification { get; }

		[Export ("objectForKey:"), Internal]
		NSObject ObjectForKey (NSString key);

		[Export ("displayNameForKey:value:"), Internal]
		NSString DisplayNameForKey (NSString key, string value);

		[Internal, Field ("NSLocaleIdentifier")]
		NSString _Identifier { get; }
		
		[Internal, Field ("NSLocaleLanguageCode")]
		NSString _LanguageCode { get; }
		
		[Internal, Field ("NSLocaleCountryCode")]
		NSString _CountryCode { get; }
		
		[Internal, Field ("NSLocaleScriptCode")]
		NSString _ScriptCode { get; }
		
		[Internal, Field ("NSLocaleVariantCode")]
		NSString _VariantCode { get; }
		
		[Internal, Field ("NSLocaleExemplarCharacterSet")]
		NSString _ExemplarCharacterSet { get; }
		
		[Internal, Field ("NSLocaleCalendar")]
		NSString _Calendar { get; }
		
		[Internal, Field ("NSLocaleCollationIdentifier")]
		NSString _CollationIdentifier { get; }
		
		[Internal, Field ("NSLocaleUsesMetricSystem")]
		NSString _UsesMetricSystem { get; }
		
		[Internal, Field ("NSLocaleMeasurementSystem")]
		NSString _MeasurementSystem { get; }
		
		[Internal, Field ("NSLocaleDecimalSeparator")]
		NSString _DecimalSeparator { get; }
		
		[Internal, Field ("NSLocaleGroupingSeparator")]
		NSString _GroupingSeparator { get; }
		
		[Internal, Field ("NSLocaleCurrencySymbol")]
		NSString _CurrencySymbol { get; }
		
		[Internal, Field ("NSLocaleCurrencyCode")]
		NSString _CurrencyCode { get; }
		
		[Internal, Field ("NSLocaleCollatorIdentifier")]
		NSString _CollatorIdentifier { get; }
		
		[Internal, Field ("NSLocaleQuotationBeginDelimiterKey")]
		NSString _QuotationBeginDelimiterKey { get; }
		
		[Internal, Field ("NSLocaleQuotationEndDelimiterKey")]
		NSString _QuotationEndDelimiterKey { get; }
		
		[Internal, Field ("NSLocaleAlternateQuotationBeginDelimiterKey")]
		NSString _AlternateQuotationBeginDelimiterKey { get; }
		
		[Internal, Field ("NSLocaleAlternateQuotationEndDelimiterKey")]
		NSString _AlternateQuotationEndDelimiterKey { get; }
	}

	public delegate void NSMatchEnumerator (NSTextCheckingResult result, NSMatchingFlags flags, ref bool stop);

	// This API surfaces NSString instead of strings, because we already have the .NET version that uses
	// strings, so it makes sense to use NSString here (and also, the replacing functionality operates on
	// NSMutableStrings)
	[BaseType (typeof (NSObject))]
	public interface NSRegularExpression : NSCopying, NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithPattern:options:error:")]
		IntPtr Constructor (NSString pattern, NSRegularExpressionOptions options, out NSError error);

		[Export ("pattern")]
		NSString Pattern { get; }

		[Export ("options")]
		NSRegularExpressionOptions Options { get; }

		[Export ("numberOfCaptureGroups")]
		nuint NumberOfCaptureGroups { get; }

		[Export ("escapedPatternForString:")]
		[Static]
		NSString GetEscapedPattern (NSString str);

		[Export ("enumerateMatchesInString:options:range:usingBlock:")]
		void EnumerateMatches (NSString str, NSMatchingOptions options, NSRange range, NSMatchEnumerator enumerator);

		[Export ("matchesInString:options:range:")]
		NSString [] GetMatches (NSString str, NSMatchingOptions options, NSRange range);

		[Export ("numberOfMatchesInString:options:range:")]
		nuint GetNumberOfMatches (NSString str, NSMatchingOptions options, NSRange range);
		
		[Export ("firstMatchInString:options:range:")]
		NSTextCheckingResult FindFirstMatch (string str, NSMatchingOptions options, NSRange range);
		
		[Export ("rangeOfFirstMatchInString:options:range:")]
		NSRange GetRangeOfFirstMatch (string str, NSMatchingOptions options, NSRange range);
		
		[Export ("stringByReplacingMatchesInString:options:range:withTemplate:")]
		string ReplaceMatches (string sourceString, NSMatchingOptions options, NSRange range, string template);
		
		[Export ("replaceMatchesInString:options:range:withTemplate:")]
		nuint ReplaceMatches (NSMutableString mutableString, NSMatchingOptions options, NSRange range,  NSString template);

		[Export ("replacementStringForResult:inString:offset:template:")]
		NSString GetReplacementString (NSTextCheckingResult result, NSString str, nint offset, NSString template);

		[Static, Export ("escapedTemplateForString:")]
		NSString GetEscapedTemplate (NSString str);
		
	}
	
	[BaseType (typeof (NSObject))]
	// init returns NIL
	[DisableDefaultCtor]
	public interface NSRunLoop {
		[Export ("currentRunLoop")][Static][IsThreadStatic]
		NSRunLoop Current { get; }

		[Export ("mainRunLoop")][Static]
		NSRunLoop Main { get; }

		[Export ("currentMode")]
		NSString CurrentMode { get; }

		[Export ("getCFRunLoop")]
		CFRunLoop GetCFRunLoop ();

		[Export ("addTimer:forMode:")]
		void AddTimer (NSTimer timer, NSString forMode);

		[Export ("limitDateForMode:")]
		NSDate LimitDateForMode (NSString mode);

		[Export ("acceptInputForMode:beforeDate:")]
		void AcceptInputForMode (NSString mode, NSDate limitDate);

		[Export ("run")]
		void Run ();

		[Export ("runUntilDate:")]
		void RunUntil (NSDate date);

		[Export ("runMode:beforeDate:")]
		bool RunUntil (NSString runLoopMode, NSDate limitdate);
		
		[Field ("NSDefaultRunLoopMode")]
		NSString NSDefaultRunLoopMode { get; }

		[Field ("NSRunLoopCommonModes")]
		NSString NSRunLoopCommonModes { get; }

		[NoiOS, NoWatch, NoTV]
		[Field ("NSConnectionReplyMode")]
		NSString NSRunLoopConnectionReplyMode { get; }

		[NoiOS, NoWatch, NoTV]
		[Field ("NSModalPanelRunLoopMode", "AppKit")]
		NSString NSRunLoopModalPanelMode { get; }

		[NoiOS, NoWatch, NoTV]
		[Field ("NSEventTrackingRunLoopMode", "AppKit")]
		NSString NSRunLoopEventTracking { get; }

		[NoMac][NoWatch]
		[Field ("UITrackingRunLoopMode", "UIKit")]
		NSString UITrackingRunLoopMode { get; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSSet : NSSecureCoding, NSMutableCopying {
		[Export ("set")][Static]
		NSSet CreateSet ();

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet other);
		
		[Export ("initWithArray:")]
		IntPtr Constructor (NSArray other);
		
		[Export ("count")]
		nuint Count { get; }

		[Internal]
		[Sealed]
		[Export ("member:")]
		IntPtr _LookupMember (IntPtr probe);

		[Export ("member:")]
		NSObject LookupMember (NSObject probe);

		[Internal]
		[Sealed]
		[Export ("anyObject")]
		IntPtr _AnyObject { get; }

		[Export ("anyObject")]
		NSObject AnyObject { get; }

		[Internal]
		[Sealed]
		[Export ("containsObject:")]
		bool _Contains (IntPtr id);

		[Export ("containsObject:")]
		bool Contains (NSObject id);

		[Export ("allObjects")][Internal]
		IntPtr _AllObjects ();

		[Export ("isEqualToSet:")]
		bool IsEqualToSet (NSSet other);

		[Export ("objectEnumerator"), Internal]
		NSEnumerator _GetEnumerator ();
		
		[Export ("isSubsetOfSet:")]
		bool IsSubsetOf (NSSet other);
		
		[Export ("enumerateObjectsUsingBlock:")]
		[Since (4,0)]
		void Enumerate (NSSetEnumerator enumerator);

		[Internal]
		[Sealed]
		[Export ("setByAddingObjectsFromSet:")]
		IntPtr _SetByAddingObjectsFromSet (IntPtr other);

		[Export ("setByAddingObjectsFromSet:"), Internal]
		NSSet SetByAddingObjectsFromSet (NSSet other);

		[Export ("intersectsSet:")]
		bool IntersectsSet (NSSet other);

		[Internal]
		[Static]
		[Export ("setWithArray:")]
		IntPtr _SetWithArray (IntPtr array);

#if MACCORE
		[Mac (10,11)]
		[Static]
		[Export ("setWithCollectionViewIndexPath:")]
		NSSet FromCollectionViewIndexPath (NSIndexPath indexPath);

		[Mac (10,11)]
		[Static]
		[Export ("setWithCollectionViewIndexPaths:")]
		NSSet FromCollectionViewIndexPaths (NSIndexPath[] indexPaths);

		[Mac (10,11)]
		[Export ("enumerateIndexPathsWithOptions:usingBlock:")]
		void Enumerate (NSEnumerationOptions opts, Action<NSIndexPath, out bool> block);
#endif
	}

	public interface NSSet<TKey> : NSSet {}

	[BaseType (typeof (NSObject))]
	public interface NSSortDescriptor : NSSecureCoding, NSCopying {
		[Export ("initWithKey:ascending:")]
		IntPtr Constructor (string key, bool ascending);

		[Export ("initWithKey:ascending:selector:")]
		IntPtr Constructor (string key, bool ascending, Selector selector);

		[Export ("initWithKey:ascending:comparator:")]
		IntPtr Constructor (string key, bool ascending, NSComparator comparator);

		[Export ("key")]
		string Key { get; }

		[Export ("ascending")]
		bool Ascending { get; }

		[Export ("selector")]
		Selector Selector { get; }

		[Export ("compareObject:toObject:")]
		NSComparisonResult Compare (NSObject object1, NSObject object2);

		[Export ("reversedSortDescriptor")]
		NSObject ReversedSortDescriptor { get; }

		[Since (7,0), Mavericks]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();
	}
	
	[Category, BaseType (typeof (NSOrderedSet))]
	public partial interface NSKeyValueSorting_NSOrderedSet {
		[Since (5,0)]
		[Export ("sortedArrayUsingDescriptors:")]
		NSObject [] GetSortedArray (NSSortDescriptor [] sortDescriptors);
	}
	
#pragma warning disable 618
	[Category, BaseType (typeof (NSMutableArray))]
#pragma warning restore 618
	public partial interface NSSortDescriptorSorting_NSMutableArray {
		[Since (5,0), Export ("sortUsingDescriptors:")]
		void SortUsingDescriptors (NSSortDescriptor [] sortDescriptors);
	}

	[Category, BaseType (typeof (NSMutableOrderedSet))]
	public partial interface NSKeyValueSorting_NSMutableOrderedSet {
		[Since (5,0), Export ("sortUsingDescriptors:")]
		void SortUsingDescriptors (NSSortDescriptor [] sortDescriptors);
	}
	
	[BaseType (typeof(NSObject))]
	[Dispose ("if (disposing) { Invalidate (); } ")]
	// init returns NIL
	[DisableDefaultCtor]
	public interface NSTimer {
		// TODO: scheduledTimerWithTimeInterval:invocation:repeats:

		[Static, Export ("scheduledTimerWithTimeInterval:target:selector:userInfo:repeats:")]
		NSTimer CreateScheduledTimer (double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		// TODO: timerWithTimeInterval:invocation:repeats:

		[Static, Export ("timerWithTimeInterval:target:selector:userInfo:repeats:")]
		NSTimer CreateTimer (double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[DesignatedInitializer]
		[Export ("initWithFireDate:interval:target:selector:userInfo:repeats:")]
		IntPtr Constructor (NSDate date, double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[Export ("fire")]
		void Fire ();

		[NullAllowed] // by default this property is null
		[Export ("fireDate", ArgumentSemantic.Copy)]
		NSDate FireDate { get; set; }

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("timeInterval")]
		double TimeInterval { get; }

		[Export ("userInfo")]
		NSObject UserInfo { get; }

		[Since (7,0), Mavericks]
		[Export ("tolerance")]
		double Tolerance { get; set; }
	}

	[BaseType (typeof(NSObject))]
	// NSTimeZone is an abstract class that defines the behavior of time zone objects. -> http://developer.apple.com/library/ios/#documentation/Cocoa/Reference/Foundation/Classes/NSTimeZone_Class/Reference/Reference.html
	// calling 'init' returns a NIL pointer, i.e. an unusable instance
	[DisableDefaultCtor]
	public interface NSTimeZone : NSSecureCoding, NSCopying {
		[Export ("initWithName:")]
		IntPtr Constructor (string name);
		
		[Export ("initWithName:data:")]
		IntPtr Constructor (string name, NSData data);

		[Export ("name")]
		string Name { get; } 

		[Export ("data")]
		NSData Data { get; }

		[Export ("secondsFromGMTForDate:")]
		nint SecondsFromGMT (NSDate date);

		[Static]
		[Export ("abbreviationDictionary")]
		NSDictionary Abbreviations { get; }

		[Export ("abbreviation")]
		string Abbreviation ();

		[Export ("abbreviationForDate:")]
		string Abbreviation (NSDate date);

		[Export ("isDaylightSavingTimeForDate:")]
		bool IsDaylightSavingsTime (NSDate date);

		[Export ("daylightSavingTimeOffsetForDate:")]
		double DaylightSavingTimeOffset (NSDate date);

		[Export ("nextDaylightSavingTimeTransitionAfterDate:")]
		NSDate NextDaylightSavingTimeTransitionAfter (NSDate date);

		[Static, Export ("timeZoneWithName:")]
		NSTimeZone FromName (string tzName);

		[Static, Export ("timeZoneWithName:data:")]
		NSTimeZone FromName (string tzName, NSData data);
		
		[Static]
		[Export ("timeZoneForSecondsFromGMT:")]
		NSTimeZone FromGMT (nint seconds);

		[Static, Export ("localTimeZone")]
		NSTimeZone LocalTimeZone { get; }

		[Export ("secondsFromGMT")]
		nint GetSecondsFromGMT { get; }

		[Export ("defaultTimeZone"), Static]
		NSTimeZone DefaultTimeZone { get; set; }

		[Export ("resetSystemTimeZone"), Static]
		void ResetSystemTimeZone ();

		[Export ("systemTimeZone"), Static]
		NSTimeZone SystemTimeZone { get; }
		
		[Export ("timeZoneWithAbbreviation:"), Static]
		NSTimeZone FromAbbreviation (string abbreviation);

		[Export ("knownTimeZoneNames"), Static, Internal]
		string[] _KnownTimeZoneNames { get; }

		[Export ("timeZoneDataVersion"), Static]
		string DataVersion { get; }

		[Export ("localizedName:locale:")]
		string GetLocalizedName (NSTimeZoneNameStyle style, NSLocale locale);
	}

	interface NSUbiquitousKeyValueStoreChangeEventArgs {
		[Export ("NSUbiquitousKeyValueStoreChangedKeysKey")]
		string [] ChangedKeys { get; }
	
		[Export ("NSUbiquitousKeyValueStoreChangeReasonKey")]
		NSUbiquitousKeyValueStoreChangeReason ChangeReason { get; }
	}

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
#if WATCH
	[DisableDefaultCtor] // "NSUbiquitousKeyValueStore is unavailable" is printed to the log.
#endif
	interface NSUbiquitousKeyValueStore {
		[Static]
		[Export ("defaultStore")]
		NSUbiquitousKeyValueStore DefaultStore { get; }

		[Export ("objectForKey:"), Internal]
		NSObject ObjectForKey (string aKey);

		[Export ("setObject:forKey:"), Internal]
		void SetObjectForKey (NSObject anObject, string aKey);

		[Export ("removeObjectForKey:")]
		void Remove (string aKey);

		[Export ("stringForKey:")]
		string GetString (string aKey);

		[Export ("arrayForKey:")]
		NSObject [] GetArray (string aKey);

		[Export ("dictionaryForKey:")]
		NSDictionary GetDictionary (string aKey);

		[Export ("dataForKey:")]
		NSData GetData (string aKey);

		[Export ("longLongForKey:")]
		long GetLong (string aKey);

		[Export ("doubleForKey:")]
		double GetDouble (string aKey);

		[Export ("boolForKey:")]
		bool GetBool (string aKey);

		[Export ("setString:forKey:"), Internal]
		void _SetString (string aString, string aKey);

		[Export ("setData:forKey:"), Internal]
		void _SetData (NSData data, string key);

		[Export ("setArray:forKey:"), Internal]
		void _SetArray (NSObject [] array, string key);

		[Export ("setDictionary:forKey:"), Internal]
		void _SetDictionary (NSDictionary aDictionary, string aKey);

		[Export ("setLongLong:forKey:"), Internal]
		void _SetLong (long value, string aKey);

		[Export ("setDouble:forKey:"), Internal]
		void _SetDouble (double value, string aKey);

		[Export ("setBool:forKey:"), Internal]
		void _SetBool (bool value, string aKey);

		[Export ("dictionaryRepresentation")]
#if XAMCORE_2_0
		NSDictionary ToDictionary ();
#else
		[Obsolete ("Use ToDictionary instead")]
		NSDictionary DictionaryRepresentation ();
#endif

		[Export ("synchronize")]
		bool Synchronize ();

		[Field ("NSUbiquitousKeyValueStoreDidChangeExternallyNotification")]
		[Notification (typeof (NSUbiquitousKeyValueStoreChangeEventArgs))]
		NSString DidChangeExternallyNotification { get; }

		[Field ("NSUbiquitousKeyValueStoreChangeReasonKey")]
		NSString ChangeReasonKey { get; }

		[Field ("NSUbiquitousKeyValueStoreChangedKeysKey")]
		NSString ChangedKeysKey { get; }
	}

	[Since (6,0)]
	[BaseType (typeof (NSObject), Name="NSUUID")]
	public interface NSUuid : NSSecureCoding, NSCopying {
		[Export ("initWithUUIDString:")]
		IntPtr Constructor (string str);

		// bound manually to keep the managed/native signatures identical
		//[Export ("initWithUUIDBytes:"), Internal]
		//IntPtr Constructor (IntPtr bytes, bool unused);

		[Export ("getUUIDBytes:"), Internal]
		void GetUuidBytes (IntPtr uuid);

		[Export ("UUIDString")]
		string AsString ();
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // .objc_class_name_NSUserActivity", referenced from '' not found
	[BaseType (typeof (NSObject))]
	public partial interface NSUserActivity {
	
		[Export ("initWithActivityType:")]
#if XAMCORE_2_0
		IntPtr Constructor (string activityType);
#else
		IntPtr Constructor (NSString activityType);
#endif

		[Export ("activityType")]
		string ActivityType { get; }
	
		[NullAllowed] // by default this property is null
		[Export ("title")]
		string Title { get; set; }
	
		[Export ("userInfo", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary UserInfo { get; set; }
	
		[Export ("needsSave")]
		bool NeedsSave { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("webpageURL", ArgumentSemantic.Copy)]
		NSUrl WebPageUrl { get; set; }
	
		[Export ("supportsContinuationStreams")]
		bool SupportsContinuationStreams { get; set; }
	
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSUserActivityDelegate Delegate { get; set; }
	
		[Export ("addUserInfoEntriesFromDictionary:")]
		void AddUserInfoEntries (NSDictionary otherDictionary);
	
		[Export ("becomeCurrent")]
		void BecomeCurrent ();
	
		[Export ("invalidate")]
		void Invalidate ();
	
		[Export ("getContinuationStreamsWithCompletionHandler:")]
		[Async (ResultTypeName="NSUserActivityContinuation")]
		void GetContinuationStreams (Action<NSInputStream,NSOutputStream,NSError> completionHandler);

		[Mac(10,11), iOS (9,0)]
		[Export ("requiredUserInfoKeys", ArgumentSemantic.Copy)]
		NSSet<NSString> RequiredUserInfoKeys { get; set; }

		[Mac(10,11), iOS (9,0)]
		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }

		[Mac(10,11), iOS (9,0)]
		[Export ("keywords", ArgumentSemantic.Copy)]
		NSSet<NSString> Keywords { get; set; }

		[Mac(10,11), iOS (9,0)]
		[Export ("resignCurrent")]
		void ResignCurrent ();

		[Mac(10,11), iOS (9,0)]
		[Export ("eligibleForHandoff")]
		bool EligibleForHandoff { [Bind ("isEligibleForHandoff")] get; set; }

		[Mac(10,11), iOS (9,0)]
		[Export ("eligibleForSearch")]
		bool EligibleForSearch { [Bind ("isEligibleForSearch")] get; set; }

		[Mac(10,11), iOS (9,0)]
		[Export ("eligibleForPublicIndexing")]
		bool EligibleForPublicIndexing { [Bind ("isEligibleForPublicIndexing")] get; set; }
		
#if IOS
		[iOS (9,0)]
		[NullAllowed]
		[Export ("contentAttributeSet", ArgumentSemantic.Copy)] // From CSSearchableItemAttributeSet.h
		CSSearchableItemAttributeSet ContentAttributeSet { get; set; }
#endif

	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // same as NSUserActivity
	[Static]
	public partial interface NSUserActivityType {
		[Field ("NSUserActivityTypeBrowsingWeb")]
		NSString BrowsingWeb { get; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // same as NSUserActivity
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	public partial interface NSUserActivityDelegate {
		[Export ("userActivityWillSave:")]
		void UserActivityWillSave (NSUserActivity userActivity);
	
		[Export ("userActivityWasContinued:")]
		void UserActivityWasContinued (NSUserActivity userActivity);
	
		[Export ("userActivity:didReceiveInputStream:outputStream:")]
		void UserActivityReceivedData (NSUserActivity userActivity, NSInputStream inputStream, NSOutputStream outputStream);
	}
		
	[BaseType (typeof (NSObject))]
	public interface NSUserDefaults {
		[Mac (10,6)][iOS (4,0)]
		[Export ("URLForKey:")]
		[return: NullAllowed]
		NSUrl URLForKey (string defaultName);

		[Mac (10,6)][iOS (4,0)]
		[Export ("setURL:forKey:")]
		void SetURL ([NullAllowed] NSUrl url, string defaultName);

		[Static]
		[Export ("standardUserDefaults")]
		NSUserDefaults StandardUserDefaults { get; }
	
		[Static]
		[Export ("resetStandardUserDefaults")]
		void ResetStandardUserDefaults ();
	
		[Internal]
		[Export ("initWithUser:")]
		IntPtr InitWithUserName (string username);

		[Internal]
		[Since (7,0), Mavericks]
		[Export ("initWithSuiteName:")]
		IntPtr InitWithSuiteName (string suiteName);

		[Export ("objectForKey:")][Internal]
		NSObject ObjectForKey (string defaultName);
	
		[Export ("setObject:forKey:")][Internal]
		void SetObjectForKey (NSObject value, string defaultName);
	
		[Export ("removeObjectForKey:")]
		void RemoveObject (string defaultName);
	
		[Export ("stringForKey:")]
		string StringForKey (string defaultName);
	
		[Export ("arrayForKey:")]
		NSObject [] ArrayForKey (string defaultName);
	
		[Export ("dictionaryForKey:")]
		NSDictionary DictionaryForKey (string defaultName);
	
		[Export ("dataForKey:")]
		NSData DataForKey (string defaultName);
	
		[Export ("stringArrayForKey:")]
		string [] StringArrayForKey (string defaultName);
	
		[Export ("integerForKey:")]
		nint IntForKey (string defaultName);
	
		[Export ("floatForKey:")]
		float FloatForKey (string defaultName); // this is defined as float, not CGFloat.
	
		[Export ("doubleForKey:")]
		double DoubleForKey (string defaultName);
	
		[Export ("boolForKey:")]
		bool BoolForKey (string defaultName);
	
		[Export ("setInteger:forKey:")]
		void SetInt (nint value, string defaultName);
	
		[Export ("setFloat:forKey:")]
		void SetFloat (float value /* this is defined as float, not CGFloat */, string defaultName);
	
		[Export ("setDouble:forKey:")]
		void SetDouble (double value, string defaultName);
	
		[Export ("setBool:forKey:")]
		void SetBool (bool value, string  defaultName);
	
		[Export ("registerDefaults:")]
		void RegisterDefaults (NSDictionary registrationDictionary);
	
		[Export ("addSuiteNamed:")]
		void AddSuite (string suiteName);
	
		[Export ("removeSuiteNamed:")]
		void RemoveSuite (string suiteName);
	
		[Export ("dictionaryRepresentation")]
#if XAMCORE_2_0
		NSDictionary ToDictionary ();
#else
		[Obsolete ("Use ToDictionary instead")]
		NSDictionary AsDictionary ();
#endif
	
		[Export ("volatileDomainNames")]
		string [] VolatileDomainNames ();
	
		[Export ("volatileDomainForName:")]
		NSDictionary GetVolatileDomain (string domainName);
	
		[Export ("setVolatileDomain:forName:")]
		void SetVolatileDomain (NSDictionary domain, string domainName);
	
		[Export ("removeVolatileDomainForName:")]
		void RemoveVolatileDomain (string domainName);
	
		[Availability (Deprecated = Platform.iOS_7_0 | Platform.Mac_10_9)]
		[Export ("persistentDomainNames")]
		string [] PersistentDomainNames ();
	
		[Export ("persistentDomainForName:")]
		NSDictionary PersistentDomainForName (string domainName);
	
		[Export ("setPersistentDomain:forName:")]
		void SetPersistentDomain (NSDictionary domain, string domainName);
	
		[Export ("removePersistentDomainForName:")]
		void RemovePersistentDomain (string domainName);
	
		[Export ("synchronize")]
		bool Synchronize ();
	
		[Export ("objectIsForcedForKey:")]
		bool ObjectIsForced (string key);
	
		[Export ("objectIsForcedForKey:inDomain:")]
		bool ObjectIsForced (string key, string domain);

		[Field ("NSGlobalDomain")]
		NSString GlobalDomain { get; }

		[Field ("NSArgumentDomain")]
		NSString ArgumentDomain { get; }

		[Field ("NSRegistrationDomain")]
		NSString RegistrationDomain { get; }

		[iOS (9,3)]
		[NoMac][NoTV]
		[Notification]
		[Field ("NSUserDefaultsSizeLimitExceededNotification")]
		NSString SizeLimitExceededNotification { get; }

		[iOS (9,3)]
		[NoMac][NoTV]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsDidChangeAccountsNotification")]
		NSString DidChangeAccountsNotification { get; }

		[iOS (9,3)]
		[NoMac][NoTV]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsCompletedInitialSyncNotification")]
		NSString CompletedInitialSyncNotification { get; }

		[Notification]
		[Field ("NSUserDefaultsDidChangeNotification")]
		NSString DidChangeNotification { get; }
	}
	
	[BaseType (typeof (NSObject), Name="NSURL")]
	// init returns NIL
	[DisableDefaultCtor]
	public partial interface NSUrl : NSSecureCoding, NSCopying
#if MONOMAC
	, NSPasteboardReading, NSPasteboardWriting
#endif
	{
		[Export ("initWithScheme:host:path:")]
		IntPtr Constructor (string scheme, string host, string path);

		[DesignatedInitializer]
		[Export ("initFileURLWithPath:isDirectory:")]
		IntPtr Constructor (string path, bool isDir);

		[Export ("initWithString:")]
		IntPtr Constructor (string urlString);

		[DesignatedInitializer]
		[Export ("initWithString:relativeToURL:")]
		IntPtr Constructor (string urlString, NSUrl relativeToUrl);

		[Export ("URLWithString:")][Static]
		NSUrl FromString (string s);

		[Export ("URLWithString:relativeToURL:")][Internal][Static]
		NSUrl _FromStringRelative (string url, NSUrl relative);
		
		[Export ("absoluteString")]
		string AbsoluteString { get; }

		[Export ("absoluteURL")]
		NSUrl AbsoluteUrl { get; }

		[Export ("baseURL")]
		NSUrl BaseUrl { get; }

		[Export ("fragment")]
		string Fragment { get; }

		[Export ("host")]
		string Host { get; }

#if XAMCORE_2_0
		[Internal]
#endif
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSUrl other);

		[Export ("isFileURL")]
		bool IsFileUrl { get; }

		[Export ("parameterString")]
		string ParameterString { get;}

		[Export ("password")]
		string Password { get;}

		[Export ("path")]
		string Path { get;}

		[Export ("query")]
		string Query { get;}

		[Export ("relativePath")]
		string RelativePath { get;}

		[Export ("pathComponents")]
		string [] PathComponents { get; }

		[Export ("lastPathComponent")]
		string LastPathComponent { get; }

		[Export ("pathExtension")]
		string PathExtension { get; }

		[Export ("relativeString")]
		string RelativeString { get;}

		[Export ("resourceSpecifier")]
		string ResourceSpecifier { get;}

		[Export ("scheme")]
		string Scheme { get;}

		[Export ("user")]
		string User { get;}

		[Export ("standardizedURL")]
		NSUrl StandardizedUrl { get; }

		[Export ("URLByAppendingPathComponent:isDirectory:")]
		NSUrl Append (string pathComponent, bool isDirectory);

		[Export ("URLByAppendingPathExtension:")]
		NSUrl AppendPathExtension (string extension);

		[Export ("URLByDeletingLastPathComponent")]
		NSUrl RemoveLastPathComponent ();

		[Export ("URLByDeletingPathExtension")]
		NSUrl RemovePathExtension ();

		[Since (7,0), Mavericks]
		[Export ("getFileSystemRepresentation:maxLength:")]
		bool GetFileSystemRepresentation (IntPtr buffer, nint maxBufferLength);

		[Since (7,0), Mavericks]
		[Export ("fileSystemRepresentation")]
		IntPtr GetFileSystemRepresentationAsUtf8Ptr { get; }

		[Since (7,0), Mavericks]
		[Export ("removeCachedResourceValueForKey:")]
		void RemoveCachedResourceValueForKey (NSString key);

		[Since (7,0), Mavericks]
		[Export ("removeAllCachedResourceValues")]
		void RemoveAllCachedResourceValues ();

		[Since (7,0), Mavericks]
		[Export ("setTemporaryResourceValue:forKey:")]
		void SetTemporaryResourceValue (NSObject value, NSString key);

		[DesignatedInitializer]
		[Since (7,0), Mavericks]
		[Export ("initFileURLWithFileSystemRepresentation:isDirectory:relativeToURL:")]
		IntPtr Constructor (IntPtr ptrUtf8path, bool isDir, NSUrl baseURL);

		[Since (7,0), Mavericks, Static, Export ("fileURLWithFileSystemRepresentation:isDirectory:relativeToURL:")]
		NSUrl FromUTF8Pointer (IntPtr ptrUtf8path, bool isDir, NSUrl baseURL);

#if MONOMAC

		/* These methods come from NURL_AppKitAdditions */

		[Export ("URLFromPasteboard:")]
		[Static]
		NSUrl FromPasteboard (NSPasteboard pasteboard);

		[Export ("writeToPasteboard:")]
		void WriteToPasteboard (NSPasteboard pasteboard);
#endif
		[Export("bookmarkDataWithContentsOfURL:error:")]
		[Static]
		NSData GetBookmarkData (NSUrl bookmarkFileUrl, out NSError error);

		[Export("URLByResolvingBookmarkData:options:relativeToURL:bookmarkDataIsStale:error:")]
		[Static]
		NSUrl FromBookmarkData (NSData data, NSUrlBookmarkResolutionOptions options, [NullAllowed] NSUrl relativeToUrl, out bool isStale, out NSError error);

		[Export("writeBookmarkData:toURL:options:error:")]
		[Static]
		bool WriteBookmarkData (NSData data, NSUrl bookmarkFileUrl, NSUrlBookmarkCreationOptions options, out NSError error);

		[Export("filePathURL")]
		NSUrl FilePathUrl { get; }

		[Export("fileReferenceURL")]
		NSUrl FileReferenceUrl { get; }		

		[Export ("getResourceValue:forKey:error:"), Internal]
		bool GetResourceValue (out NSObject value, NSString key, out NSError error);

		[Export ("resourceValuesForKeys:error:")]
		NSDictionary GetResourceValues (NSString [] keys, out NSError error);

		[Export ("setResourceValue:forKey:error:"), Internal]
		bool SetResourceValue (NSObject value, NSString key, out NSError error);
		
		[Export ("port"), Internal]
		[NullAllowed]
		NSNumber PortNumber { get; }

		[Field ("NSURLNameKey")]
		NSString NameKey { get; }

		[Field ("NSURLLocalizedNameKey")]
		NSString LocalizedNameKey { get; }

		[Field ("NSURLIsRegularFileKey")]
		NSString IsRegularFileKey { get; }

		[Field ("NSURLIsDirectoryKey")]
		NSString IsDirectoryKey { get; }

		[Field ("NSURLIsSymbolicLinkKey")]
		NSString IsSymbolicLinkKey { get; }

		[Field ("NSURLIsVolumeKey")]
		NSString IsVolumeKey { get; }

		[Field ("NSURLIsPackageKey")]
		NSString IsPackageKey { get; }

		[Field ("NSURLIsSystemImmutableKey")]
		NSString IsSystemImmutableKey { get; }

		[Field ("NSURLIsUserImmutableKey")]
		NSString IsUserImmutableKey { get; }

		[Field ("NSURLIsHiddenKey")]
		NSString IsHiddenKey { get; }

		[Field ("NSURLHasHiddenExtensionKey")]
		NSString HasHiddenExtensionKey { get; }

		[Field ("NSURLCreationDateKey")]
		NSString CreationDateKey { get; }

		[Field ("NSURLContentAccessDateKey")]
		NSString ContentAccessDateKey { get; }

		[Field ("NSURLContentModificationDateKey")]
		NSString ContentModificationDateKey { get; }

		[Field ("NSURLAttributeModificationDateKey")]
		NSString AttributeModificationDateKey { get; }

		[Field ("NSURLLinkCountKey")]
		NSString LinkCountKey { get; }

		[Field ("NSURLParentDirectoryURLKey")]
		NSString ParentDirectoryURLKey { get; }

		[Field ("NSURLVolumeURLKey")]
		NSString VolumeURLKey { get; }

		[Field ("NSURLTypeIdentifierKey")]
		NSString TypeIdentifierKey { get; }

		[Field ("NSURLLocalizedTypeDescriptionKey")]
		NSString LocalizedTypeDescriptionKey { get; }

		[Field ("NSURLLabelNumberKey")]
		NSString LabelNumberKey { get; }

		[Field ("NSURLLabelColorKey")]
		NSString LabelColorKey { get; }

		[Field ("NSURLLocalizedLabelKey")]
		NSString LocalizedLabelKey { get; }

		[Field ("NSURLEffectiveIconKey")]
		NSString EffectiveIconKey { get; }

		[Field ("NSURLCustomIconKey")]
		NSString CustomIconKey { get; }

		[Field ("NSURLFileSizeKey")]
		NSString FileSizeKey { get; }

		[Field ("NSURLFileAllocatedSizeKey")]
		NSString FileAllocatedSizeKey { get; }

		[Field ("NSURLIsAliasFileKey")]
		NSString IsAliasFileKey	{ get; }

		[Field ("NSURLVolumeLocalizedFormatDescriptionKey")]
		NSString VolumeLocalizedFormatDescriptionKey { get; }

		[Field ("NSURLVolumeTotalCapacityKey")]
		NSString VolumeTotalCapacityKey { get; }

		[Field ("NSURLVolumeAvailableCapacityKey")]
		NSString VolumeAvailableCapacityKey { get; }

		[Field ("NSURLVolumeResourceCountKey")]
		NSString VolumeResourceCountKey { get; }

		[Field ("NSURLVolumeSupportsPersistentIDsKey")]
		NSString VolumeSupportsPersistentIDsKey { get; }

		[Field ("NSURLVolumeSupportsSymbolicLinksKey")]
		NSString VolumeSupportsSymbolicLinksKey { get; }

		[Field ("NSURLVolumeSupportsHardLinksKey")]
		NSString VolumeSupportsHardLinksKey { get; }

		[Field ("NSURLVolumeSupportsJournalingKey")]
		NSString VolumeSupportsJournalingKey { get; }

		[Field ("NSURLVolumeIsJournalingKey")]
		NSString VolumeIsJournalingKey { get; }

		[Field ("NSURLVolumeSupportsSparseFilesKey")]
		NSString VolumeSupportsSparseFilesKey { get; }

		[Field ("NSURLVolumeSupportsZeroRunsKey")]
		NSString VolumeSupportsZeroRunsKey { get; }

		[Field ("NSURLVolumeSupportsCaseSensitiveNamesKey")]
		NSString VolumeSupportsCaseSensitiveNamesKey { get; }

		[Field ("NSURLVolumeSupportsCasePreservedNamesKey")]
		NSString VolumeSupportsCasePreservedNamesKey { get; }

		// 5.0 Additions
		[Since (5,0)]
		[Field ("NSURLKeysOfUnsetValuesKey")]
		NSString KeysOfUnsetValuesKey { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceIdentifierKey")]
		NSString FileResourceIdentifierKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIdentifierKey")]
		NSString VolumeIdentifierKey { get; }

		[Since (5,0)]
		[Field ("NSURLPreferredIOBlockSizeKey")]
		NSString PreferredIOBlockSizeKey { get; }

		[Since (5,0)]
		[Field ("NSURLIsReadableKey")]
		NSString IsReadableKey { get; }

		[Since (5,0)]
		[Field ("NSURLIsWritableKey")]
		NSString IsWritableKey { get; }

		[Since (5,0)]
		[Field ("NSURLIsExecutableKey")]
		NSString IsExecutableKey { get; }

		[Since (5,0)]
		[Field ("NSURLIsMountTriggerKey")]
		NSString IsMountTriggerKey { get; }

		[Since (5,0)]
		[Field ("NSURLFileSecurityKey")]
		NSString FileSecurityKey { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeKey")]
		NSString FileResourceTypeKey { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeNamedPipe")]
		NSString FileResourceTypeNamedPipe { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeCharacterSpecial")]
		NSString FileResourceTypeCharacterSpecial { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeDirectory")]
		NSString FileResourceTypeDirectory { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeBlockSpecial")]
		NSString FileResourceTypeBlockSpecial { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeRegular")]
		NSString FileResourceTypeRegular { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeSymbolicLink")]
		NSString FileResourceTypeSymbolicLink { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeSocket")]
		NSString FileResourceTypeSocket { get; }

		[Since (5,0)]
		[Field ("NSURLFileResourceTypeUnknown")]
		NSString FileResourceTypeUnknown { get; }

		[Since (5,0)]
		[Field ("NSURLTotalFileSizeKey")]
		NSString TotalFileSizeKey { get; }

		[Since (5,0)]
		[Field ("NSURLTotalFileAllocatedSizeKey")]
		NSString TotalFileAllocatedSizeKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeSupportsRootDirectoryDatesKey")]
		NSString VolumeSupportsRootDirectoryDatesKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeSupportsVolumeSizesKey")]
		NSString VolumeSupportsVolumeSizesKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeSupportsRenamingKey")]
		NSString VolumeSupportsRenamingKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeSupportsAdvisoryFileLockingKey")]
		NSString VolumeSupportsAdvisoryFileLockingKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeSupportsExtendedSecurityKey")]
		NSString VolumeSupportsExtendedSecurityKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIsBrowsableKey")]
		NSString VolumeIsBrowsableKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeMaximumFileSizeKey")]
		NSString VolumeMaximumFileSizeKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIsEjectableKey")]
		NSString VolumeIsEjectableKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIsRemovableKey")]
		NSString VolumeIsRemovableKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIsInternalKey")]
		NSString VolumeIsInternalKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIsAutomountedKey")]
		NSString VolumeIsAutomountedKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIsLocalKey")]
		NSString VolumeIsLocalKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeIsReadOnlyKey")]
		NSString VolumeIsReadOnlyKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeCreationDateKey")]
		NSString VolumeCreationDateKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeURLForRemountingKey")]
		NSString VolumeURLForRemountingKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeUUIDStringKey")]
		NSString VolumeUUIDStringKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeNameKey")]
		NSString VolumeNameKey { get; }

		[Since (5,0)]
		[Field ("NSURLVolumeLocalizedNameKey")]
		NSString VolumeLocalizedNameKey { get; }

		[Since (5,0)]
		[Field ("NSURLIsUbiquitousItemKey")]
		NSString IsUbiquitousItemKey { get; }

		[Since (5,0)]
		[Field ("NSURLUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		[Since (5,0)]
		[Field ("NSURLUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		[Field ("NSURLUbiquitousItemIsDownloadingKey")]
		[Availability (Introduced = Platform.iOS_5_0, Deprecated = Platform.iOS_7_0)]
		NSString UbiquitousItemIsDownloadingKey { get; }

		[Since (5,0)]
		[Field ("NSURLUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		[Since (5,0)]
		[Field ("NSURLUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		[Field ("NSURLUbiquitousItemPercentDownloadedKey")]
		[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_7, Deprecated = Platform.iOS_6_0 | Platform.Mac_10_8, Message = "NSMetadataQuery.UbiquitousItemPercentDownloadedKey on NSMetadataItem instead")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_7, Deprecated = Platform.iOS_6_0 | Platform.Mac_10_8, Message = "NSMetadataQuery.UbiquitousItemPercentUploadedKey on NSMetadataItem instead")]
		[Field ("NSURLUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		[Since (5,1)]
		[MountainLion]
		[Field ("NSURLIsExcludedFromBackupKey")]
		NSString IsExcludedFromBackupKey { get; }

		[Export ("bookmarkDataWithOptions:includingResourceValuesForKeys:relativeToURL:error:")]
		NSData CreateBookmarkData (NSUrlBookmarkCreationOptions options, string [] resourceValues, [NullAllowed] NSUrl relativeUrl, out NSError error);

		[Export ("initByResolvingBookmarkData:options:relativeToURL:bookmarkDataIsStale:error:")]
		IntPtr Constructor (NSData bookmarkData, NSUrlBookmarkResolutionOptions resolutionOptions, [NullAllowed] NSUrl relativeUrl, out bool bookmarkIsStale, out NSError error);

		[Field ("NSURLPathKey")]
		[Since (6,0)][MountainLion]
		NSString PathKey { get; }

		[Since (7,0), Mavericks]
		[Field ("NSURLUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		[Since (7,0), Mavericks]
		[Field ("NSURLUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		[Since (7,0), Mavericks]
		[Field ("NSURLUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		[Since (7,0), Mavericks]
		[Field ("NSURLUbiquitousItemDownloadingStatusNotDownloaded")]
		NSString UbiquitousItemDownloadingStatusNotDownloaded { get; }

		[Since (7,0), Mavericks]
		[Field ("NSURLUbiquitousItemDownloadingStatusDownloaded")]
		NSString UbiquitousItemDownloadingStatusDownloaded { get; }

		[Since (7,0), Mavericks]
		[Field ("NSURLUbiquitousItemDownloadingStatusCurrent")]
		NSString UbiquitousItemDownloadingStatusCurrent { get; }

		[Mac (10,7), iOS (8,0)]
		[Export ("startAccessingSecurityScopedResource")]
		bool StartAccessingSecurityScopedResource ();

		[Mac (10,7), iOS (8,0)]
		[Export ("stopAccessingSecurityScopedResource")]
		void StopAccessingSecurityScopedResource ();

		[Mac (10,10), iOS (8,0)]
		[Static, Export ("URLByResolvingAliasFileAtURL:options:error:")]
		NSUrl ResolveAlias  (NSUrl aliasFileUrl, NSUrlBookmarkResolutionOptions options, out NSError error);

		[Static, Export ("fileURLWithPathComponents:")]
		NSUrl CreateFileUrl (string [] pathComponents);

		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLAddedToDirectoryDateKey")]
		NSString AddedToDirectoryDateKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLDocumentIdentifierKey")]
		NSString DocumentIdentifierKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLGenerationIdentifierKey")]
		NSString GenerationIdentifierKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLThumbnailDictionaryKey")]
		NSString ThumbnailDictionaryKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLUbiquitousItemDownloadRequestedKey")]
		NSString UbiquitousItemDownloadRequestedKey { get; }

		//
		// iOS 9.0/osx 10.11 additions
		//
		[DesignatedInitializer]
		[iOS (9,0), Mac(10,11)]
		[Export ("initFileURLWithPath:isDirectory:relativeToURL:")]
		IntPtr Constructor (string path, bool isDir, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("fileURLWithPath:isDirectory:relativeToURL:")]
		NSUrl CreateFileUrl (string path, bool isDir, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("fileURLWithPath:relativeToURL:")]
		NSUrl CreateFileUrl (string path, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("URLWithDataRepresentation:relativeToURL:")]
		NSUrl CreateWithDataRepresentation (NSData data, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("absoluteURLWithDataRepresentation:relativeToURL:")]
		NSUrl CreateAbsoluteUrlWithDataRepresentation (NSData data, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Export ("dataRepresentation", ArgumentSemantic.Copy)]
		NSData DataRepresentation { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("hasDirectoryPath")]
		bool HasDirectoryPath { get; }

		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLIsApplicationKey")]
		NSString IsApplicationKey { get; }

#if !MONOMAC
		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionKey")]
		NSString FileProtectionKey { get; }

		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionNone")]
		NSString FileProtectionNone { get; }
		
		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionComplete")]
		NSString FileProtectionComplete { get; }
		
		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication { get; }
#endif
	}

	
	//
	// Just a category so we can document the three methods together
	//
	[Category, BaseType (typeof (NSUrl))]
	public partial interface NSUrl_PromisedItems {
		[Mac (10,10), iOS (8,0)]
		[Export ("checkPromisedItemIsReachableAndReturnError:")]
		bool CheckPromisedItemIsReachable (out NSError error);

		[Mac (10,10), iOS (8,0)]
		[Export ("getPromisedItemResourceValue:forKey:error:")]
		bool GetPromisedItemResourceValue (out NSObject value, NSString key, out NSError error);

		[Mac (10,10), iOS (8,0)]
		[Export ("promisedItemResourceValuesForKeys:error:")]
		NSDictionary GetPromisedItemResourceValues (NSString [] keys, out NSError error);
		
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSObject), Name="NSURLQueryItem")]
	public interface NSUrlQueryItem : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithName:value:")]
		IntPtr Constructor (string name, string value);

		[Export ("name")]
		string Name { get; }

		[Export ("value")]
		string Value { get; }
	}

	[Category, BaseType (typeof (NSCharacterSet))]
	public partial interface NSUrlUtilities_NSCharacterSet {
		[Since (7,0), Static, Export ("URLUserAllowedCharacterSet")]
		NSCharacterSet UrlUserAllowedCharacterSet { get; }
	
		[Since (7,0), Static, Export ("URLPasswordAllowedCharacterSet")]
		NSCharacterSet UrlPasswordAllowedCharacterSet { get; }
	
		[Since (7,0), Static, Export ("URLHostAllowedCharacterSet")]
		NSCharacterSet UrlHostAllowedCharacterSet { get; }
	
		[Since (7,0), Static, Export ("URLPathAllowedCharacterSet")]
		NSCharacterSet UrlPathAllowedCharacterSet { get; }
	
		[Since (7,0), Static, Export ("URLQueryAllowedCharacterSet")]
		NSCharacterSet UrlQueryAllowedCharacterSet { get; }
	
		[Since (7,0), Static, Export ("URLFragmentAllowedCharacterSet")]
		NSCharacterSet UrlFragmentAllowedCharacterSet { get; }
	}
		
	[BaseType (typeof (NSObject), Name="NSURLCache")]
	public interface NSUrlCache {
		[Export ("sharedURLCache"), Static]
		NSUrlCache SharedCache { get; set; }

		[Export ("initWithMemoryCapacity:diskCapacity:diskPath:")]
		IntPtr Constructor (nuint memoryCapacity, nuint diskCapacity, string diskPath);

		[Export ("cachedResponseForRequest:")]
		NSCachedUrlResponse CachedResponseForRequest (NSUrlRequest request);

		[Export ("storeCachedResponse:forRequest:")]
		void StoreCachedResponse (NSCachedUrlResponse cachedResponse, NSUrlRequest forRequest);

		[Export ("removeCachedResponseForRequest:")]
		void RemoveCachedResponse (NSUrlRequest request);

		[Export ("removeAllCachedResponses")]
		void RemoveAllCachedResponses ();

		[Export ("memoryCapacity")]
		nuint MemoryCapacity { get; set; }

		[Export ("diskCapacity")]
		nuint DiskCapacity { get; set; }

		[Export ("currentMemoryUsage")]
		nuint CurrentMemoryUsage { get; }

		[Export ("currentDiskUsage")]
		nuint CurrentDiskUsage { get; }

		[Mac(10,10)][iOS(8,0)]
		[Export ("removeCachedResponsesSinceDate:")]
		void RemoveCachedResponsesSinceDate (NSDate date);

		[iOS (8,0), Mac(10,10)]
		[Export ("storeCachedResponse:forDataTask:")]
		void StoreCachedResponse (NSCachedUrlResponse cachedResponse, NSUrlSessionDataTask dataTask);

		[iOS (8,0), Mac(10,10)]
		[Export ("getCachedResponseForDataTask:completionHandler:")]
		[Async]
		void GetCachedResponse (NSUrlSessionDataTask dataTask, Action<NSCachedUrlResponse> completionHandler);

		[iOS (8,0), Mac(10,10)]
		[Export ("removeCachedResponseForDataTask:")]
		void RemoveCachedResponse (NSUrlSessionDataTask dataTask);
	}
	
	[Since (7,0), Mavericks]
	[BaseType (typeof (NSObject), Name="NSURLComponents")]
	public partial interface NSUrlComponents : NSCopying {
		[Export ("initWithURL:resolvingAgainstBaseURL:")]
		IntPtr Constructor (NSUrl url, bool resolveAgainstBaseUrl);
	
		[Static, Export ("componentsWithURL:resolvingAgainstBaseURL:")]
		NSUrlComponents FromUrl (NSUrl url, bool resolvingAgainstBaseUrl);
	
		[Export ("initWithString:")]
		IntPtr Constructor (string urlString);
	
		[Static, Export ("componentsWithString:")]
		NSUrlComponents FromString (string urlString);
	
		[Export ("URL")]
		NSUrl Url { get; }
	
		[Export ("URLRelativeToURL:")]
		NSUrl GetRelativeUrl (NSUrl baseUrl);
	
		[NullAllowed] // by default this property is null
		[Export ("scheme", ArgumentSemantic.Copy)]
		string Scheme { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("user", ArgumentSemantic.Copy)]
		string User { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("password", ArgumentSemantic.Copy)]
		string Password { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("host", ArgumentSemantic.Copy)]
		string Host { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("port", ArgumentSemantic.Copy)]
		NSNumber Port { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("path", ArgumentSemantic.Copy)]
		string Path { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("query", ArgumentSemantic.Copy)]
		string Query { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("fragment", ArgumentSemantic.Copy)]
		string Fragment { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedUser", ArgumentSemantic.Copy)]
		string PercentEncodedUser { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedPassword", ArgumentSemantic.Copy)]
		string PercentEncodedPassword { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedHost", ArgumentSemantic.Copy)]
		string PercentEncodedHost { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedPath", ArgumentSemantic.Copy)]
		string PercentEncodedPath { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedQuery", ArgumentSemantic.Copy)]
		string PercentEncodedQuery { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedFragment", ArgumentSemantic.Copy)]
		string PercentEncodedFragment { get; set; }

		[Mac (10,10), iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("queryItems")]
		NSUrlQueryItem [] QueryItems { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("string")]
		string AsString ();

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfScheme"), Mac(10,11)]
		NSRange RangeOfScheme { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfUser"), Mac(10,11)]
		NSRange RangeOfUser { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfPassword"), Mac(10,11)]
		NSRange RangeOfPassword { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfHost"), Mac(10,11)]
		NSRange RangeOfHost { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfPort"), Mac(10,11)]
		NSRange RangeOfPort { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfPath"), Mac(10,11)]
		NSRange RangeOfPath { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfQuery"), Mac(10,11)]
		NSRange RangeOfQuery { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfFragment"), Mac(10,11)]
		NSRange RangeOfFragment { get; }
	}
	
	[BaseType (typeof (NSObject), Name="NSURLAuthenticationChallenge")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	public interface NSUrlAuthenticationChallenge : NSSecureCoding {
		[Export ("initWithProtectionSpace:proposedCredential:previousFailureCount:failureResponse:error:sender:")]
		IntPtr Constructor (NSUrlProtectionSpace space, NSUrlCredential credential, nint previousFailureCount, NSUrlResponse response, [NullAllowed] NSError error, NSUrlConnection sender);
		
		[Export ("initWithAuthenticationChallenge:sender:")]
		IntPtr Constructor (NSUrlAuthenticationChallenge  challenge, NSUrlConnection sender);
	
		[Export ("protectionSpace")]
		NSUrlProtectionSpace ProtectionSpace { get; }
	
		[Export ("proposedCredential")]
		NSUrlCredential ProposedCredential { get; }
	
		[Export ("previousFailureCount")]
		nint PreviousFailureCount { get; }
	
		[Export ("failureResponse")]
		NSUrlResponse FailureResponse { get; }
	
		[Export ("error")]
		NSError Error { get; }
	
		[Export ("sender")]
		NSUrlConnection Sender { get; }
	}

	[Protocol (Name = "NSURLAuthenticationChallengeSender")]
#if XAMCORE_4_0
	public interface NSUrlAuthenticationChallengeSender {
#else
	[Model]
	[BaseType (typeof (NSObject))]
	public interface NSURLAuthenticationChallengeSender {
#endif
		[Abstract]
		[Export ("useCredential:forAuthenticationChallenge:")]
#if XAMCORE_2_0
		void UseCredential (NSUrlCredential credential, NSUrlAuthenticationChallenge challenge);
#else
		void UseCredentials (NSUrlCredential credential, NSUrlAuthenticationChallenge challenge);
#endif

		[Abstract]
		[Export ("continueWithoutCredentialForAuthenticationChallenge:")]
#if XAMCORE_2_0
		void ContinueWithoutCredential (NSUrlAuthenticationChallenge challenge);
#else
		void ContinueWithoutCredentialForAuthenticationChallenge (NSUrlAuthenticationChallenge challenge);
#endif

		[Abstract]
		[Export ("cancelAuthenticationChallenge:")]
		void CancelAuthenticationChallenge (NSUrlAuthenticationChallenge challenge);

		[iOS (5,0)]
		[Export ("performDefaultHandlingForAuthenticationChallenge:")]
#if XAMCORE_2_0
		void PerformDefaultHandling (NSUrlAuthenticationChallenge challenge);
#else
		[Abstract]
		void PerformDefaultHandlingForChallenge (NSUrlAuthenticationChallenge challenge);
#endif

		[iOS (5,0)]
		[Export ("rejectProtectionSpaceAndContinueWithChallenge:")]
#if XAMCORE_2_0
		void RejectProtectionSpaceAndContinue (NSUrlAuthenticationChallenge challenge);
#else
		[Abstract]
		void RejectProtectionSpaceAndContinueWithChallenge (NSUrlAuthenticationChallenge challenge);
#endif
	}


	public delegate void NSUrlConnectionDataResponse (NSUrlResponse response, NSData data, NSError error);
	
	[BaseType (typeof (NSObject), Name="NSURLConnection")]
	public interface NSUrlConnection : 
#if XAMCORE_4_0
		NSUrlAuthenticationChallengeSender
#else
		NSURLAuthenticationChallengeSender
#endif
	{
		[Export ("canHandleRequest:")][Static]
		bool CanHandleRequest (NSUrlRequest request);
	
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9,0, message: "Use NSUrlSession instead")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use NSUrlSession instead")]
		[Export ("connectionWithRequest:delegate:")][Static]
		NSUrlConnection FromRequest (NSUrlRequest request, [Protocolize] NSUrlConnectionDelegate connectionDelegate);
	
		[Deprecated (PlatformName.iOS, 9,0, message: "Use NSUrlSession instead")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use NSUrlSession instead")]
		[Export ("initWithRequest:delegate:")]
		IntPtr Constructor (NSUrlRequest request, [Protocolize] NSUrlConnectionDelegate connectionDelegate);
	
		[Deprecated (PlatformName.iOS, 9,0, message: "Use NSUrlSession instead")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use NSUrlSession instead")]
		[Export ("initWithRequest:delegate:startImmediately:")]
		IntPtr Constructor (NSUrlRequest request, [Protocolize] NSUrlConnectionDelegate connectionDelegate, bool startImmediately);
	
		[Export ("start")]
		void Start ();
	
		[Export ("cancel")]
		void Cancel ();
	
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);
	
		[Export ("unscheduleFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);

#if !MONOMAC
		[Since (5,0)]
		[Export ("originalRequest")]
		NSUrlRequest OriginalRequest { get; }

		[Since (5,0)]
		[Export ("currentRequest")]
		NSUrlRequest CurrentRequest { get; }
#endif
		[Export ("setDelegateQueue:")]
		[Since (5,0)]
		void SetDelegateQueue (NSOperationQueue queue);

		[NoWatch]
		[Since (5,0)]
		[Static]
		[Export ("sendAsynchronousRequest:queue:completionHandler:")]
		[Async (ResultTypeName = "NSUrlAsyncResult", MethodName="SendRequestAsync")]
		void SendAsynchronousRequest (NSUrlRequest request, NSOperationQueue queue, NSUrlConnectionDataResponse completionHandler);
		
#if IOS
		// Extension from iOS5, NewsstandKit
		[Since (5,0)]
		[Export ("newsstandAssetDownload", ArgumentSemantic.Weak)]
		XamCore.NewsstandKit.NKAssetDownload NewsstandAssetDownload { get; }
#endif
	}

	[BaseType (typeof (NSObject), Name="NSURLConnectionDelegate")]
	[Model]
	[Protocol]
	public interface NSUrlConnectionDelegate {
#if !XAMCORE_2_0
		// part of NSURLConnectionDataDelegate
		[Export ("connection:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlConnection connection, NSUrlRequest request, NSUrlResponse response);

		[Export ("connection:needNewBodyStream:")]
		NSInputStream NeedNewBodyStream (NSUrlConnection connection, NSUrlRequest request);
#endif

		[Export ("connection:canAuthenticateAgainstProtectionSpace:")]
		[Availability (Deprecated=Platform.iOS_8_0|Platform.Mac_10_10, Message="Use WillSendRequestForAuthenticationChallenge instead")]
		bool CanAuthenticateAgainstProtectionSpace (NSUrlConnection connection, NSUrlProtectionSpace protectionSpace);

		[Export ("connection:didReceiveAuthenticationChallenge:")]
		[Availability (Deprecated=Platform.iOS_8_0|Platform.Mac_10_10, Message="Use WillSendRequestForAuthenticationChallenge instead")]
		void ReceivedAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		[Export ("connection:didCancelAuthenticationChallenge:")]
		[Availability (Deprecated=Platform.iOS_8_0|Platform.Mac_10_10, Message="Use WillSendRequestForAuthenticationChallenge instead")]
		void CanceledAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		[Export ("connectionShouldUseCredentialStorage:")]
		bool ConnectionShouldUseCredentialStorage (NSUrlConnection connection);

#if !XAMCORE_2_0
		// part of NSURLConnectionDataDelegate
		[Export ("connection:didReceiveResponse:")]
		void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response);

		[Export ("connection:didReceiveData:")]
		void ReceivedData (NSUrlConnection connection, NSData data);

		[Export ("connection:didSendBodyData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void SentBodyData (NSUrlConnection connection, nint bytesWritten, nint totalBytesWritten, nint totalBytesExpectedToWrite);

		[Export ("connection:willCacheResponse:")]
		NSCachedUrlResponse WillCacheResponse (NSUrlConnection connection, NSCachedUrlResponse cachedResponse);

		[Export ("connectionDidFinishLoading:")]
		void FinishedLoading (NSUrlConnection connection);
#endif

		[Export ("connection:didFailWithError:")]
		void FailedWithError (NSUrlConnection connection, NSError error);

		[Export ("connection:willSendRequestForAuthenticationChallenge:")]
		void WillSendRequestForAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);
	}

#if XAMCORE_2_0
	[BaseType (typeof (NSUrlConnectionDelegate), Name="NSURLConnectionDataDelegate")]
	[Protocol, Model]
	interface NSUrlConnectionDataDelegate {

		[Export ("connection:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlConnection connection, NSUrlRequest request, NSUrlResponse response);

		[Export ("connection:didReceiveResponse:")]
		void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response);

		[Export ("connection:didReceiveData:")]
		void ReceivedData (NSUrlConnection connection, NSData data);

		[Export ("connection:needNewBodyStream:")]
		NSInputStream NeedNewBodyStream (NSUrlConnection connection, NSUrlRequest request);

		[Export ("connection:didSendBodyData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void SentBodyData (NSUrlConnection connection, nint bytesWritten, nint totalBytesWritten, nint totalBytesExpectedToWrite);

		[Export ("connection:willCacheResponse:")]
		NSCachedUrlResponse WillCacheResponse (NSUrlConnection connection, NSCachedUrlResponse cachedResponse);

		[Export ("connectionDidFinishLoading:")]
		void FinishedLoading (NSUrlConnection connection);
	}
#endif

	[BaseType (typeof (NSUrlConnectionDelegate), Name="NSURLConnectionDownloadDelegate")]
	[Model]
	[Protocol]
	public interface NSUrlConnectionDownloadDelegate {
		[Export ("connection:didWriteData:totalBytesWritten:expectedTotalBytes:")]
		void WroteData (NSUrlConnection connection, long bytesWritten, long totalBytesWritten, long expectedTotalBytes);
		
		[Export ("connectionDidResumeDownloading:totalBytesWritten:expectedTotalBytes:")]
		void ResumedDownloading (NSUrlConnection connection, long totalBytesWritten, long expectedTotalBytes);
		
		[Abstract]
		[Export ("connectionDidFinishDownloading:destinationURL:")]
		void FinishedDownloading (NSUrlConnection connection, NSUrl destinationUrl);
	}
		
	[BaseType (typeof (NSObject), Name="NSURLCredential")]
	// crash when calling NSObjecg.get_Description (and likely other selectors)
	[DisableDefaultCtor]
	public interface NSUrlCredential : NSSecureCoding, NSCopying {

		[Export ("initWithTrust:")]
		IntPtr Constructor (SecTrust trust);

		[Export ("persistence")]
		NSUrlCredentialPersistence Persistence { get; }

		[Export ("initWithUser:password:persistence:")]
		IntPtr Constructor (string user, string password, NSUrlCredentialPersistence persistence);
	
		[Static]
		[Export ("credentialWithUser:password:persistence:")]
		NSUrlCredential FromUserPasswordPersistance (string user, string password, NSUrlCredentialPersistence persistence);

		[Export ("user")]
		string User { get; }
	
		[Export ("password")]
		string Password { get; }
	
		[Export ("hasPassword")]
		bool HasPassword {get; }
	
		[Export ("initWithIdentity:certificates:persistence:")]
		[Internal]
		IntPtr Constructor (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistance);
	
		[Static]
		[Internal]
		[Export ("credentialWithIdentity:certificates:persistence:")]
		NSUrlCredential FromIdentityCertificatesPersistanceInternal (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistence);
	
#if XAMCORE_2_0
		[Internal]
#else
		[Obsolete ("Use SecIdentity property")]
#endif
		[Export ("identity")]
		IntPtr Identity { get; }
	
		[Export ("certificates")]
		SecCertificate [] Certificates { get; }
	
		// bound manually to keep the managed/native signatures identical
		//[Export ("initWithTrust:")]
		//IntPtr Constructor (IntPtr SecTrustRef_trust, bool ignored);
	
#if XAMCORE_2_0
		[Internal]
#else
		[Obsolete ("Use NSUrlCredential(SecTrust) constructor")]
#endif
		[Static]
		[Export ("credentialForTrust:")]
		NSUrlCredential FromTrust (IntPtr trust);
	}

	[BaseType (typeof (NSObject), Name="NSURLCredentialStorage")]
	// init returns NIL -> SharedCredentialStorage
	[DisableDefaultCtor]
	public interface NSUrlCredentialStorage {
		[Static]
		[Export ("sharedCredentialStorage")]
		NSUrlCredentialStorage SharedCredentialStorage { get; }

		[Export ("credentialsForProtectionSpace:")]
		NSDictionary GetCredentials (NSUrlProtectionSpace forProtectionSpace);

		[Export ("allCredentials")]
		NSDictionary AllCredentials { get; }

		[Export ("setCredential:forProtectionSpace:")]
		void SetCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace);

		[Export ("removeCredential:forProtectionSpace:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace);

		[Export ("defaultCredentialForProtectionSpace:")]
		NSUrlCredential GetDefaultCredential (NSUrlProtectionSpace forProtectionSpace);

		[Export ("setDefaultCredential:forProtectionSpace:")]
		void SetDefaultCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace);

		[Since (7,0), Mavericks]
		[Export ("removeCredential:forProtectionSpace:options:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace, NSDictionary options);

		[Since (7,0), Mavericks]
		[Field ("NSURLCredentialStorageRemoveSynchronizableCredentials")]
		NSString RemoveSynchronizableCredentials { get; }

		[Field ("NSURLCredentialStorageChangedNotification")]
		[Notification]
		NSString ChangedNotification { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("getCredentialsForProtectionSpace:task:completionHandler:")]
		void GetCredentials (NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task, [NullAllowed] Action<NSDictionary> completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("setCredential:forProtectionSpace:task:")]
		void SetCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);

		[iOS (8,0), Mac (10,10)]
		[Export ("removeCredential:forProtectionSpace:options:task:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSDictionary options, NSUrlSessionTask task);

		[iOS (8,0), Mac (10,10)]
		[Export ("getDefaultCredentialForProtectionSpace:task:completionHandler:")]
		void GetDefaultCredential (NSUrlProtectionSpace space, NSUrlSessionTask task, [NullAllowed] Action<NSUrlCredential> completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("setDefaultCredential:forProtectionSpace:task:")]
		void SetDefaultCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);
		
	}

#if XAMCORE_4_0
	public delegate void NSUrlSessionPendingTasks (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#elif XAMCORE_3_0
	public delegate void NSUrlSessionPendingTasks2 (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#else
	public delegate void NSUrlSessionPendingTasks (NSUrlSessionDataTask [] dataTasks, NSUrlSessionUploadTask [] uploadTasks, NSUrlSessionDownloadTask[] downloadTasks);
	public delegate void NSUrlSessionPendingTasks2 (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#endif
	public delegate void NSUrlSessionAllPendingTasks (NSUrlSessionTask [] tasks);
	public delegate void NSUrlSessionResponse (NSData data, NSUrlResponse response, NSError error);
	public delegate void NSUrlSessionDownloadResponse (NSUrl data, NSUrlResponse response, NSError error);

	public delegate void NSUrlDownloadSessionResponse (NSUrl location, NSUrlResponse response, NSError error);

	//
	// Some of the XxxTaskWith methods that take a completion were flagged as allowing a null in
	// 083d9cba1eb997eac5c5ded77db32180c3eef566 with comment:
	//
	// "Add missing [NullAllowed] on NSUrlSession since the
	// delegate is optional and the handler can be null when one
	// is provided (but requiring a delegate along with handlers
	// only duplicates code)"
	//
	// but Apple has flagged these as not allowing null.
	//
	// Leaving the null allowed for now.
	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)] // 64-bit on 10.9, but 32/64-bit on 10.10
	[BaseType (typeof (NSObject), Name="NSURLSession")]
#if XAMCORE_2_0
	[DisableDefaultCtorAttribute]
#endif
	public partial interface NSUrlSession {
	
		[Static, Export ("sharedSession")]
		NSUrlSession SharedSession { get; }
	
		[Static, Export ("sessionWithConfiguration:")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration);
	
		[Static, Export ("sessionWithConfiguration:delegate:delegateQueue:")]
		NSUrlSession FromWeakConfiguration (NSUrlSessionConfiguration configuration, [NullAllowed] NSObject weakDelegate, [NullAllowed] NSOperationQueue delegateQueue);
	
		[Static, Wrap ("FromWeakConfiguration (configuration, sessionDelegate, delegateQueue);")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration, NSUrlSessionDelegate sessionDelegate, NSOperationQueue delegateQueue);
	
		[Export ("delegateQueue", ArgumentSemantic.Retain)]
		NSOperationQueue DelegateQueue { get; }
	
		[Export ("delegate", ArgumentSemantic.Retain), NullAllowed]
		NSObject WeakDelegate { get; }
	
		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSUrlSessionDelegate Delegate { get; }
	
		[Export ("configuration", ArgumentSemantic.Copy)]
		NSUrlSessionConfiguration Configuration { get; }

		[NullAllowed]
		[Export ("sessionDescription", ArgumentSemantic.Copy)]
		string SessionDescription { get; set; }
	
		[Export ("finishTasksAndInvalidate")]
		void FinishTasksAndInvalidate ();
	
		[Export ("invalidateAndCancel")]
		void InvalidateAndCancel ();
	
		[Export ("resetWithCompletionHandler:")]
		[Async]
		void Reset (NSAction completionHandler);
	
		[Export ("flushWithCompletionHandler:")]
		[Async]
		void Flush (NSAction completionHandler);
	
#if !XAMCORE_3_0
		// broken version that we must keep for XAMCORE_2_0 binary compatibility
		// but that we do not have to expose on tvOS and watchOS, forcing people to use the correct API
		[Obsolete ("Use GetTasks2 instead. This method may throw spurious InvalidCastExceptions, in particular for backgrounded tasks.")]
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionActiveTasks")]
		void GetTasks (NSUrlSessionPendingTasks completionHandler);
#elif XAMCORE_4_0
		// Fixed version (breaking change) only for XAMCORE_4_0
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionActiveTasks")]
		void GetTasks (NSUrlSessionPendingTasks completionHandler);
#endif

#if !XAMCORE_4_0
		// Workaround, not needed for XAMCORE_4_0+
		[Sealed]
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionActiveTasks2")]
		void GetTasks2 (NSUrlSessionPendingTasks2 completionHandler);
#endif

		[Export ("dataTaskWithRequest:")]
		NSUrlSessionDataTask CreateDataTask (NSUrlRequest request);
	
		[Export ("dataTaskWithURL:")]
		NSUrlSessionDataTask CreateDataTask (NSUrl url);
	
		[Export ("uploadTaskWithRequest:fromFile:")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL);
	
		[Export ("uploadTaskWithRequest:fromData:")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData);
	
		[Export ("uploadTaskWithStreamedRequest:")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request);
	
		[Export ("downloadTaskWithRequest:")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request);
	
		[Export ("downloadTaskWithURL:")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url);
	
		[Export ("downloadTaskWithResumeData:")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSData resumeData);

		[Export ("dataTaskWithRequest:completionHandler:")]
		[Async (ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDataTask CreateDataTask (NSUrlRequest request, [NullAllowed] NSUrlSessionResponse completionHandler);
	
		[Export ("dataTaskWithURL:completionHandler:")]
		[Async(ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDataTask CreateDataTask (NSUrl url, [NullAllowed] NSUrlSessionResponse completionHandler);
	
		[Export ("uploadTaskWithRequest:fromFile:completionHandler:")]
		[Async(ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL, NSUrlSessionResponse completionHandler);
	
		[Export ("uploadTaskWithRequest:fromData:completionHandler:")]
		[Async(ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData, NSUrlSessionResponse completionHandler);
	
		[Export ("downloadTaskWithRequest:completionHandler:")]
		[Async(ResultTypeName="NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);
	
		[Export ("downloadTaskWithURL:completionHandler:")]
		[Async(ResultTypeName="NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

		[Export ("downloadTaskWithResumeData:completionHandler:")]
		[Async(ResultTypeName="NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTaskFromResumeData (NSData resumeData, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

        
		[iOS (9,0), Mac(10,11)]
		[Export ("getAllTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionCombinedTasks")]
		void GetAllTasks (NSUrlSessionAllPendingTasks completionHandler);

		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Export ("streamTaskWithHostName:port:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (string hostname, nint port);

		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Export ("streamTaskWithNetService:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (NSNetService service);
	}

	[Since(9,0)]
	[Protocol, Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name="NSURLSessionStreamDelegate")]
	public interface NSUrlSessionStreamDelegate
	{
		[Export ("URLSession:readClosedForStreamTask:")]
		void ReadClosed (NSUrlSession session, NSUrlSessionStreamTask streamTask);
	
		[Export ("URLSession:writeClosedForStreamTask:")]
		void WriteClosed (NSUrlSession session, NSUrlSessionStreamTask streamTask);
	
		[Export ("URLSession:betterRouteDiscoveredForStreamTask:")]
		void BetterRouteDiscovered (NSUrlSession session, NSUrlSessionStreamTask streamTask);
	
		//
		// Note: the names of this methods do not exactly match the Objective-C name
		// because it was a bad name, and does not describe what this does, so the name
		// was picked from the documentation and what it does.
		//
		[Export ("URLSession:streamTask:didBecomeInputStream:outputStream:")]
		void CompletedTaskCaptureStreams (NSUrlSession session, NSUrlSessionStreamTask streamTask, NSInputStream inputStream, NSOutputStream outputStream);
	}
	
	public delegate void NSUrlSessionDataRead (NSData data, bool atEof, NSError error);
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSUrlSessionTask), Name="NSURLSessionStreamTask")]
	[DisableDefaultCtor]
	public interface NSUrlSessionStreamTask
	{
		[Export ("readDataOfMinLength:maxLength:timeout:completionHandler:")]
		[Async (ResultTypeName="NSUrlSessionStreamDataRead")]
		void ReadData (nuint minBytes, nuint maxBytes, double timeout, NSUrlSessionDataRead completionHandler);
	
		[Export ("writeData:timeout:completionHandler:")]
		[Async]
		void WriteData (NSData data, double timeout, Action<NSError> completionHandler);
	
		[Export ("captureStreams")]
		void CaptureStreams ();
	
		[Export ("closeWrite")]
		void CloseWrite ();
	
		[Export ("closeRead")]
		void CloseRead ();
	
		[Export ("startSecureConnection")]
		void StartSecureConnection ();
	
		[Export ("stopSecureConnection")]
		void StopSecureConnection ();
	}
	
	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSObject), Name="NSURLSessionTask")]
	public partial interface NSUrlSessionTask : NSCopying {
	
		[Export ("taskIdentifier")]
		nuint TaskIdentifier { get; }
	
		[Export ("originalRequest", ArgumentSemantic.Copy), NullAllowed]
		NSUrlRequest OriginalRequest { get; }
	
		[Export ("currentRequest", ArgumentSemantic.Copy), NullAllowed]
		NSUrlRequest CurrentRequest { get; }
	
		[Export ("response", ArgumentSemantic.Copy), NullAllowed]
		NSUrlResponse Response { get; }
	
		[Export ("countOfBytesReceived")]
		long BytesReceived { get; }
	
		[Export ("countOfBytesSent")]
		long BytesSent { get; }
	
		[Export ("countOfBytesExpectedToSend")]
		long BytesExpectedToSend { get; }
	
		[Export ("countOfBytesExpectedToReceive")]
		long BytesExpectedToReceive { get; }
	
		[NullAllowed] // by default this property is null
		[Export ("taskDescription", ArgumentSemantic.Copy)]
		string TaskDescription { get; set; }
	
		[Export ("cancel")]
		void Cancel ();
	
		[Export ("state")]
		NSUrlSessionTaskState State { get; }
	
		[Export ("error", ArgumentSemantic.Copy), NullAllowed]
		NSError Error { get; }
	
		[Export ("suspend")]
		void Suspend ();
	
		[Export ("resume")]
		void Resume ();

		[Field ("NSURLSessionTransferSizeUnknown")]
		long TransferSizeUnknown { get; }

#if !MONOMAC || XAMCORE_2_0
		[iOS (8,0), Mac (10,10, onlyOn64 : true)]
		[Export ("priority")]
		float Priority { get; set; } /* float, not CGFloat */
#endif
	}

	[Static]
	[iOS (8,0)]
	[Mac (10,10)]
#if XAMCORE_2_0
	interface NSUrlSessionTaskPriority {
#else
	interface NSURLSessionTaskPriority {
#endif
		[Field ("NSURLSessionTaskPriorityDefault")]
		float Default { get; } /* float, not CGFloat */
		
		[Field ("NSURLSessionTaskPriorityLow")]
		float Low { get; } /* float, not CGFloat */
		
		[Field ("NSURLSessionTaskPriorityHigh")]
		float High { get; } /* float, not CGFloat */
	}
	
	// All of the NSUrlSession APIs are either 10.10, or 10.9 and 64-bit only
	// "NSURLSession is not available for i386 targets before Mac OS X 10.10."

	//
	// Empty interfaces, just to distinguish semantically their usage
	//
	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSUrlSessionTask), Name="NSURLSessionDataTask")]
	public partial interface NSUrlSessionDataTask {}

	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSUrlSessionDataTask), Name="NSURLSessionUploadTask")]
	public partial interface NSUrlSessionUploadTask {}

	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSUrlSessionTask), Name="NSURLSessionDownloadTask")]
	public partial interface NSUrlSessionDownloadTask {
		[Export ("cancelByProducingResumeData:")]
		void Cancel (Action<NSData> resumeCallback);
	}
	

	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSObject), Name="NSURLSessionConfiguration")]
#if XAMCORE_2_0
	[DisableDefaultCtorAttribute]
#endif
	public partial interface NSUrlSessionConfiguration : NSCopying {
	
		[Static, Export ("defaultSessionConfiguration")]
		NSUrlSessionConfiguration DefaultSessionConfiguration { get; }
	
		[Static, Export ("ephemeralSessionConfiguration")]
		NSUrlSessionConfiguration EphemeralSessionConfiguration { get; }
	
		[Static, Export ("backgroundSessionConfiguration:")]
		[Availability (Deprecated = Platform.iOS_8_0 | Platform.Mac_10_10, Message = "Use CreateBackgroundSessionConfiguration instead")]
		NSUrlSessionConfiguration BackgroundSessionConfiguration (string identifier);
	
		[Export ("identifier", ArgumentSemantic.Copy), NullAllowed]
		string Identifier { get; }
	
		[Export ("requestCachePolicy")]
		NSUrlRequestCachePolicy RequestCachePolicy { get; set; }
	
		[Export ("timeoutIntervalForRequest")]
		double TimeoutIntervalForRequest { get; set; }
	
		[Export ("timeoutIntervalForResource")]
		double TimeoutIntervalForResource { get; set; }
	
		[Export ("networkServiceType")]
		NSUrlRequestNetworkServiceType NetworkServiceType { get; set; }
	
		[Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; set; }
	
		[Export ("discretionary")]
		bool Discretionary { [Bind ("isDiscretionary")] get; set; }
	
		[Export ("sessionSendsLaunchEvents")]
		bool SessionSendsLaunchEvents { get; set; }

		[NullAllowed]
		[Export ("connectionProxyDictionary", ArgumentSemantic.Copy)]
		NSDictionary ConnectionProxyDictionary { get; set; }
	
		[Export ("TLSMinimumSupportedProtocol")]
		SslProtocol TLSMinimumSupportedProtocol { get; set; }
	
		[Export ("TLSMaximumSupportedProtocol")]
		SslProtocol TLSMaximumSupportedProtocol { get; set; }
	
		[Export ("HTTPShouldUsePipelining")]
		bool HttpShouldUsePipelining { get; set; }
	
		[Export ("HTTPShouldSetCookies")]
		bool HttpShouldSetCookies { get; set; }
	
		[Export ("HTTPCookieAcceptPolicy")]
		NSHttpCookieAcceptPolicy HttpCookieAcceptPolicy { get; set; }
	
		[NullAllowed]
		[Export ("HTTPAdditionalHeaders", ArgumentSemantic.Copy)]
		NSDictionary HttpAdditionalHeaders { get; set; }
	
		[Export ("HTTPMaximumConnectionsPerHost")]
		nint HttpMaximumConnectionsPerHost { get; set; }
	
		[NullAllowed]
		[Export ("HTTPCookieStorage", ArgumentSemantic.Retain)]
		NSHttpCookieStorage HttpCookieStorage { get; set; }
	
		[NullAllowed]
		[Export ("URLCredentialStorage", ArgumentSemantic.Retain)]
		NSUrlCredentialStorage URLCredentialStorage { get; set; }

		[NullAllowed]
		[Export ("URLCache", ArgumentSemantic.Retain)]
		NSUrlCache URLCache { get; set; }
	
		[NullAllowed]
		[Export ("protocolClasses", ArgumentSemantic.Copy)]
		NSArray WeakProtocolClasses { get; set; }

		[NullAllowed]
		[iOS (8,0), Mac (10,10)]
		[Export ("sharedContainerIdentifier")]
		string SharedContainerIdentifier { get; set; }

		[iOS (8,0)]
		[Static, Export ("backgroundSessionConfigurationWithIdentifier:")]
		NSUrlSessionConfiguration CreateBackgroundSessionConfiguration (string identifier);

		[iOS (9,0), Mac(10,11)]
		[Export ("shouldUseExtendedBackgroundIdleMode")]
		bool ShouldUseExtendedBackgroundIdleMode { get; set; }
	}

	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model, BaseType (typeof (NSObject), Name="NSURLSessionDelegate")]
	[Protocol]
	public partial interface NSUrlSessionDelegate {
		[Export ("URLSession:didBecomeInvalidWithError:")]
		void DidBecomeInvalid (NSUrlSession session, NSError error);
	
		[Export ("URLSession:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition,NSUrlCredential> completionHandler);
	
		[Export ("URLSessionDidFinishEventsForBackgroundURLSession:")]
		void DidFinishEventsForBackgroundSession (NSUrlSession session);
	}

	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model]
	[BaseType (typeof (NSUrlSessionDelegate), Name="NSURLSessionTaskDelegate")]
	[Protocol]
	public partial interface NSUrlSessionTaskDelegate {
	
		[Export ("URLSession:task:willPerformHTTPRedirection:newRequest:completionHandler:")]
		void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler);
	
		[Export ("URLSession:task:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition,NSUrlCredential> completionHandler);
	
		[Export ("URLSession:task:needNewBodyStream:")]
		void NeedNewBodyStream (NSUrlSession session, NSUrlSessionTask task, Action<NSInputStream> completionHandler);
	
		[Export ("URLSession:task:didSendBodyData:totalBytesSent:totalBytesExpectedToSend:")]
		void DidSendBodyData (NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend);
	
		[Export ("URLSession:task:didCompleteWithError:")]
		void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error);
	}
	
	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name="NSURLSessionDataDelegate")]
	[Protocol]
	public partial interface NSUrlSessionDataDelegate {
		[Export ("URLSession:dataTask:didReceiveResponse:completionHandler:")]
		void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler);
	
		[Export ("URLSession:dataTask:didBecomeDownloadTask:")]
		void DidBecomeDownloadTask (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionDownloadTask downloadTask);
	
		[Export ("URLSession:dataTask:didReceiveData:")]
		void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data);
	
		[Export ("URLSession:dataTask:willCacheResponse:completionHandler:")]
		void WillCacheResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler);

		[iOS(9,0), Mac(10,11)]
		[Export ("URLSession:dataTask:didBecomeStreamTask:")]
		void DidBecomeStreamTask (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionStreamTask streamTask);
	}
	
	[Since (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name="NSURLSessionDownloadDelegate")]
	[Protocol]
	public partial interface NSUrlSessionDownloadDelegate {
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("URLSession:downloadTask:didFinishDownloadingToURL:")]
		void DidFinishDownloading (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location);
	
		[Export ("URLSession:downloadTask:didWriteData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void DidWriteData (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite);
	
		[Export ("URLSession:downloadTask:didResumeAtOffset:expectedTotalBytes:")]
		void DidResume (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long resumeFileOffset, long expectedTotalBytes);
	
		[Field ("NSURLSessionDownloadTaskResumeData")]
		NSString TaskResumeDataKey { get; }
	}

	interface NSUndoManagerCloseUndoGroupEventArgs {
		// Bug in docs, see header file
		[Export ("NSUndoManagerGroupIsDiscardableKey")]
		[NullAllowed]
		bool Discardable { get; }
	}
	
	[BaseType (typeof (NSObject))]
	public interface NSUndoManager {
		[Export ("beginUndoGrouping")]
		void BeginUndoGrouping ();
		
		[Export ("endUndoGrouping")]
		void EndUndoGrouping ();
		
		[Export ("groupingLevel")]
		nint GroupingLevel { get; }
		
		[Export ("disableUndoRegistration")]
		void DisableUndoRegistration ();

		[Export ("enableUndoRegistration")]
		void EnableUndoRegistration ();

		[Export ("isUndoRegistrationEnabled")]
		bool IsUndoRegistrationEnabled { get; }
		
		[Export ("groupsByEvent")]
		bool GroupsByEvent { get; set; }
		
		[Export ("levelsOfUndo")]
		nint LevelsOfUndo { get; set; }
		
		[Export ("runLoopModes")]
		string [] RunLoopModes { get; set; } 
		
		[Export ("undo")]
		void Undo ();
		
		[Export ("redo")]
		void Redo ();
		
		[Export ("undoNestedGroup")]
		void UndoNestedGroup ();
		
		[Export ("canUndo")]
		bool CanUndo { get; }
		
		[Export ("canRedo")]
		bool CanRedo { get; }

		[Export ("isUndoing")]
		bool IsUndoing { get; }

		[Export ("isRedoing")]
		bool IsRedoing { get; }

		[Export ("removeAllActions")]
		void RemoveAllActions ();

		[Export ("removeAllActionsWithTarget:")]
		void RemoveAllActions (NSObject target);

		[Export ("registerUndoWithTarget:selector:object:")]
		void RegisterUndoWithTarget (NSObject target, Selector selector, [NullAllowed] NSObject anObject);

		[Export ("prepareWithInvocationTarget:")]
		NSObject PrepareWithInvocationTarget (NSObject target);

		[Export ("undoActionName")]
		string UndoActionName { get; }

		[Export ("redoActionName")]
		string RedoActionName { get; }

		[Advice ("Use correctly named method: SetActionName")]
		[Export ("setActionName:")]
		void SetActionname (string actionName); 

		[Export ("undoMenuItemTitle")]
		string UndoMenuItemTitle { get; }

		[Export ("redoMenuItemTitle")]
		string RedoMenuItemTitle { get; }

		[Export ("undoMenuTitleForUndoActionName:")]
		string UndoMenuTitleForUndoActionName (string name);

		[Export ("redoMenuTitleForUndoActionName:")]
		string RedoMenuTitleForUndoActionName (string name);

		[Field ("NSUndoManagerCheckpointNotification")]
		[Notification]
		NSString CheckpointNotification { get; }

		[Field ("NSUndoManagerDidOpenUndoGroupNotification")]
		[Notification]
		NSString DidOpenUndoGroupNotification { get; }

		[Field ("NSUndoManagerDidRedoChangeNotification")]
		[Notification]
		NSString DidRedoChangeNotification { get; }

		[Field ("NSUndoManagerDidUndoChangeNotification")]
		[Notification]
		NSString DidUndoChangeNotification { get; }

		[Field ("NSUndoManagerWillCloseUndoGroupNotification")]
		[Notification (typeof (NSUndoManagerCloseUndoGroupEventArgs))]
		NSString WillCloseUndoGroupNotification { get; }

		[Field ("NSUndoManagerWillRedoChangeNotification")]
		[Notification]
		NSString WillRedoChangeNotification { get; }

		[Field ("NSUndoManagerWillUndoChangeNotification")]
		[Notification]
		NSString WillUndoChangeNotification { get; }

		[Since (5,0)]
		[Export ("setActionIsDiscardable:")]
		void SetActionIsDiscardable (bool discardable);

		[Since (5,0)]
		[Export ("undoActionIsDiscardable")]
		bool UndoActionIsDiscardable { get; }

		[Since (5,0)]
		[Export ("redoActionIsDiscardable")]
		bool RedoActionIsDiscardable { get; }

		[Since (5,0)]
		[Field ("NSUndoManagerGroupIsDiscardableKey")]
		NSString GroupIsDiscardableKey { get; }

		[Since (5,0)]
		[Field ("NSUndoManagerDidCloseUndoGroupNotification")]
		[Notification (typeof (NSUndoManagerCloseUndoGroupEventArgs))]
		NSString DidCloseUndoGroupNotification { get; }

	    [iOS (9,0), Mac(10,11)]
		[Export ("registerUndoWithTarget:handler:")]
		void RegisterUndo (NSObject target, Action<NSObject> undoHandler);

	}
	
	[BaseType (typeof (NSObject), Name="NSURLProtectionSpace")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	public interface NSUrlProtectionSpace : NSSecureCoding, NSCopying {
		
		[Internal]
		[Export ("initWithHost:port:protocol:realm:authenticationMethod:")]
		IntPtr Init (string host, nint port, [NullAllowed] string protocol, [NullAllowed] string realm, [NullAllowed] string authenticationMethod);
	
		[Internal]
		[Export ("initWithProxyHost:port:type:realm:authenticationMethod:")]
		IntPtr InitWithProxy (string host, nint port, [NullAllowed] string type, [NullAllowed] string realm, [NullAllowed] string authenticationMethod);
	
		[Export ("realm")]
		string Realm { get; }
	
		[Export ("receivesCredentialSecurely")]
		bool ReceivesCredentialSecurely { get; }
	
		[Export ("isProxy")]
		bool IsProxy { get; }
	
		[Export ("host")]
		string Host { get; }
	
		[Export ("port")]
		nint  Port { get; }
	
		[Export ("proxyType")]
		string ProxyType { get; }
	
		[Export ("protocol")]
		string Protocol { get; }
	
		[Export ("authenticationMethod")]
		string AuthenticationMethod { get; }

		// NSURLProtectionSpace(NSClientCertificateSpace)

		[Export ("distinguishedNames")]
		NSData [] DistinguishedNames { get; }
		
		// NSURLProtectionSpace(NSServerTrustValidationSpace)
#if XAMCORE_2_0
		[Internal]
#else
		[Obsolete ("Use ServerSecTrust")]
#endif
		[Export ("serverTrust")]
		IntPtr ServerTrust { get ; }

		[Field ("NSURLProtectionSpaceHTTP")]
		NSString HTTP { get; }

		[Field ("NSURLProtectionSpaceHTTPS")]
		NSString HTTPS { get; }

		[Field ("NSURLProtectionSpaceFTP")]
		NSString FTP { get; }

		[Field ("NSURLProtectionSpaceHTTPProxy")]
		NSString HTTPProxy { get; }

		[Field ("NSURLProtectionSpaceHTTPSProxy")]
		NSString HTTPSProxy { get; }

		[Field ("NSURLProtectionSpaceFTPProxy")]
		NSString FTPProxy { get; }

		[Field ("NSURLProtectionSpaceSOCKSProxy")]
		NSString SOCKSProxy { get; }

		[Field ("NSURLAuthenticationMethodDefault")]
		NSString AuthenticationMethodDefault { get; }

		[Field ("NSURLAuthenticationMethodHTTPBasic")]
		NSString AuthenticationMethodHTTPBasic { get; }

		[Field ("NSURLAuthenticationMethodHTTPDigest")]
		NSString AuthenticationMethodHTTPDigest { get; }

		[Field ("NSURLAuthenticationMethodHTMLForm")]
		NSString AuthenticationMethodHTMLForm { get; }

		[Field ("NSURLAuthenticationMethodNTLM")]
		NSString AuthenticationMethodNTLM { get; }

		[Field ("NSURLAuthenticationMethodNegotiate")]
		NSString AuthenticationMethodNegotiate { get; }

		[Field ("NSURLAuthenticationMethodClientCertificate")]
		NSString AuthenticationMethodClientCertificate { get; }

		[Field ("NSURLAuthenticationMethodServerTrust")]
		NSString AuthenticationMethodServerTrust { get; }
	}
	
	[BaseType (typeof (NSObject), Name="NSURLRequest")]
	public interface NSUrlRequest : NSSecureCoding, NSMutableCopying {
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[DesignatedInitializer]
		[Export ("initWithURL:cachePolicy:timeoutInterval:")]
		IntPtr Constructor (NSUrl url, NSUrlRequestCachePolicy cachePolicy, double timeoutInterval);

		[Export ("requestWithURL:")][Static]
		NSUrlRequest FromUrl (NSUrl url);

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("cachePolicy")]
		NSUrlRequestCachePolicy CachePolicy { get; }

		[Export ("timeoutInterval")]
		double TimeoutInterval { get; }

		[Export ("mainDocumentURL")]
		NSUrl MainDocumentURL { get; }

		[Export ("networkServiceType")]
		NSUrlRequestNetworkServiceType NetworkServiceType { get; }

		[Since (6,0)]
		[Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; }
		
		[Export ("HTTPMethod")]
		string HttpMethod { get; }

		[Export ("allHTTPHeaderFields")]
		NSDictionary Headers { get; }

		[Internal][Export ("valueForHTTPHeaderField:")]
		string Header (string field);

		[Export ("HTTPBody")]
		NSData Body { get; }

		[Export ("HTTPBodyStream")]
		NSInputStream BodyStream { get; }

		[Export ("HTTPShouldHandleCookies")]
		bool ShouldHandleCookies { get; }
	}

	[BaseType (typeof (NSDictionary))]
	public interface NSMutableDictionary {
		[Export ("dictionaryWithContentsOfFile:")]
		[Static]
		NSMutableDictionary FromFile (string path);

		[Export ("dictionaryWithContentsOfURL:")]
		[Static]
		NSMutableDictionary FromUrl (NSUrl url);

		[Export ("dictionaryWithObject:forKey:")]
		[Static]
		NSMutableDictionary FromObjectAndKey (NSObject obj, NSObject key);

		[Export ("dictionaryWithDictionary:")]
		[Static,New]
		NSMutableDictionary FromDictionary (NSDictionary source);

		[Export ("dictionaryWithObjects:forKeys:count:")]
		[Static, Internal]
		NSMutableDictionary FromObjectsAndKeysInternalCount (NSArray objects, NSArray keys, nint count);

		[Export ("dictionaryWithObjects:forKeys:")]
		[Static, Internal, New]
		NSMutableDictionary FromObjectsAndKeysInternal (NSArray objects, NSArray Keys);
		
		[Export ("initWithDictionary:")]
		IntPtr Constructor (NSDictionary other);

		[Export ("initWithContentsOfFile:")]
		IntPtr Constructor (string fileName);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Internal]
		[Export ("initWithObjects:forKeys:")]
		IntPtr Constructor (NSArray objects, NSArray keys);

		[Export ("removeAllObjects"), Internal]
		void RemoveAllObjects ();

		[Sealed]
		[Internal]
		[Export ("removeObjectForKey:")]
		void _RemoveObjectForKey (IntPtr key);

		[Export ("removeObjectForKey:"), Internal]
		void RemoveObjectForKey (NSObject key);

		[Sealed]
		[Internal]
		[Export ("setObject:forKey:")]
		void _SetObject (IntPtr obj, IntPtr key);

		[Export ("setObject:forKey:"), Internal]
		void SetObject (NSObject obj, NSObject key);

		[Since (6,0)]
		[Static]
		[Export ("dictionaryWithSharedKeySet:")]
		NSDictionary FromSharedKeySet (NSObject sharedKeyToken);
	}

	[BaseType (typeof (NSSet))]
	public interface NSMutableSet {
		[Export ("initWithArray:")]
		IntPtr Constructor (NSArray other);

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet other);
		
		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		IntPtr Constructor (nint capacity);

		[Internal]
		[Sealed]
		[Export ("addObject:")]
		void _Add (IntPtr obj);

		[Export ("addObject:")]
		void Add (NSObject nso);

		[Internal]
		[Sealed]
		[Export ("removeObject:")]
		void _Remove (IntPtr nso);

		[Export ("removeObject:")]
		void Remove (NSObject nso);

		[Export ("removeAllObjects")]
		void RemoveAll ();

		[Internal]
		[Sealed]
		[Export ("addObjectsFromArray:")]
		void _AddObjects (IntPtr objects);

		[Export ("addObjectsFromArray:")]
		void AddObjects (NSObject [] objects);

		[Internal, Export ("minusSet:")]
		void MinusSet (NSSet other);

		[Internal, Export ("unionSet:")]
		void UnionSet (NSSet other);
	}
	
	[BaseType (typeof (NSUrlRequest), Name="NSMutableURLRequest")]
	public interface NSMutableUrlRequest {
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("initWithURL:cachePolicy:timeoutInterval:")]
		IntPtr Constructor (NSUrl url, NSUrlRequestCachePolicy cachePolicy, double timeoutInterval);

		[NullAllowed] // by default this property is null
		[New][Export ("URL")]
		NSUrl Url { get; set; }

		[New][Export ("cachePolicy")]
		NSUrlRequestCachePolicy CachePolicy { get; set; }

		[New][Export ("timeoutInterval")]
		double TimeoutInterval { set; get; }

		[NullAllowed] // by default this property is null
		[New][Export ("mainDocumentURL")]
		NSUrl MainDocumentURL { get; set; }

		[New][Export ("HTTPMethod")]
		string HttpMethod { get; set; }

		[NullAllowed] // by default this property is null
		[New][Export ("allHTTPHeaderFields")]
		NSDictionary Headers { get; set; }

		[Internal][Export ("setValue:forHTTPHeaderField:")]
		void _SetValue (string value, string field);

		[NullAllowed] // by default this property is null
		[New][Export ("HTTPBody")]
		NSData Body { get; set; }

		[NullAllowed] // by default this property is null
		[New][Export ("HTTPBodyStream")]
		NSInputStream BodyStream { get; set; }

		[New][Export ("HTTPShouldHandleCookies")]
		bool ShouldHandleCookies { get; set; }

		[Export ("networkServiceType")]
		NSUrlRequestNetworkServiceType NetworkServiceType { set; get; }

		[Since (6,0)]
		[New] [Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; set; }
	}
	
	[BaseType (typeof (NSObject), Name="NSURLResponse")]
	public interface NSUrlResponse : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithURL:MIMEType:expectedContentLength:textEncodingName:")]
		IntPtr Constructor (NSUrl url, string mimetype, nint expectedContentLength, [NullAllowed] string textEncodingName);

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("MIMEType")]
		string MimeType { get; }

		[Export ("expectedContentLength")]
		long ExpectedContentLength { get; }

		[Export ("textEncodingName")]
		string TextEncodingName { get; }

		[Export ("suggestedFilename")]
		string SuggestedFilename { get; }
	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSStreamDelegate)} )]
	public interface NSStream {
		[Export ("open")]
		void Open ();

		[Export ("close")]
		void Close ();
	
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSStreamDelegate Delegate { get; set; }

		[Export ("propertyForKey:"), Internal]
		NSObject PropertyForKey (NSString key);
	
		[Export ("setProperty:forKey:"), Internal]
		bool SetPropertyForKey ([NullAllowed] NSObject property, NSString key);
	
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, string mode);
	
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, string mode);
	
		[Export ("streamStatus")]
		NSStreamStatus Status { get; }
	
		[Export ("streamError")]
		NSError Error { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelKey")]
		NSString SocketSecurityLevelKey { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelNone")]
		NSString SocketSecurityLevelNone { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelSSLv2")]
		NSString SocketSecurityLevelSslV2 { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelSSLv3")]
		NSString SocketSecurityLevelSslV3 { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelTLSv1")]
		NSString SocketSecurityLevelTlsV1 { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelNegotiatedSSL")]
		NSString SocketSecurityLevelNegotiatedSsl { get; }

		[Advanced, Field ("NSStreamSOCKSProxyConfigurationKey")]
		NSString SocksProxyConfigurationKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyHostKey")]
		NSString SocksProxyHostKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyPortKey")]
		NSString SocksProxyPortKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyVersionKey")]
		NSString SocksProxyVersionKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyUserKey")]
		NSString SocksProxyUserKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyPasswordKey")]
		NSString SocksProxyPasswordKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyVersion4")]
		NSString SocksProxyVersion4 { get; }

		[Advanced, Field ("NSStreamSOCKSProxyVersion5")]
		NSString SocksProxyVersion5 { get; }

		[Advanced, Field ("NSStreamDataWrittenToMemoryStreamKey")]
		NSString DataWrittenToMemoryStreamKey { get; }

		[Advanced, Field ("NSStreamFileCurrentOffsetKey")]
		NSString FileCurrentOffsetKey { get; }

		[Advanced, Field ("NSStreamSocketSSLErrorDomain")]
		NSString SocketSslErrorDomain { get; }

		[Advanced, Field ("NSStreamSOCKSErrorDomain")]
		NSString SocksErrorDomain { get; }

		[Advanced, Field ("NSStreamNetworkServiceType")]
		NSString NetworkServiceType { get; }

		[Advanced, Field ("NSStreamNetworkServiceTypeVoIP")]
		NSString NetworkServiceTypeVoIP { get; }

		[Since (5,0)]
		[Advanced, Field ("NSStreamNetworkServiceTypeVideo")]
		NSString NetworkServiceTypeVideo { get; }

		[Since (5,0)]
		[Advanced, Field ("NSStreamNetworkServiceTypeBackground")]
		NSString NetworkServiceTypeBackground { get; }

		[Since (5,0)]
		[Advanced, Field ("NSStreamNetworkServiceTypeVoice")]
		NSString NetworkServiceTypeVoice { get; }

		[iOS (8,0), Mac(10,10)]
		[Static, Export ("getBoundStreamsWithBufferSize:inputStream:outputStream:")]
		void GetBoundStreams (nuint bufferSize, out NSInputStream inputStream, out NSOutputStream outputStream);

		[NoWatch]
		[iOS (8,0), Mac (10,10)]
		[Static, Export ("getStreamsToHostWithName:port:inputStream:outputStream:")]
		void GetStreamsToHost (string hostname, nint port, out NSInputStream inputStream, out NSOutputStream outputStream);		
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSStreamDelegate {
		[Export ("stream:handleEvent:"), EventArgs ("NSStream"), EventName ("OnEvent")]
		void HandleEvent (NSStream theStream, NSStreamEvent streamEvent);
	}

	[BaseType (typeof (NSObject)), Bind ("NSString")]
	public interface NSString2 : NSSecureCoding, NSMutableCopying
	#if MONOMAC
		, NSPasteboardReading, NSPasteboardWriting // Documented that it implements NSPasteboard protocols even if header doesn't show it
	#endif
	{
		[Export ("initWithData:encoding:")]
		IntPtr Constructor (NSData data, NSStringEncoding encoding);
#if MONOMAC
		[Bind ("sizeWithAttributes:")]
		CGSize StringSize ([NullAllowed] NSDictionary attributedStringAttributes);
		
		[Bind ("boundingRectWithSize:options:attributes:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options, NSDictionary attributes);
		
		[Bind ("drawAtPoint:withAttributes:")]
		void DrawString (CGPoint point, NSDictionary attributes);
		
		[Bind ("drawInRect:withAttributes:")]
		void DrawString (CGRect rect, NSDictionary attributes);
		
		[Bind ("drawWithRect:options:attributes:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, NSDictionary attributes);
#else
#if !XAMCORE_2_0
		[Bind ("sizeWithFont:")]
		//[Obsolete ("Deprecated in iOS7.   Use NSString.GetSizeUsingAttributes (UIStringAttributes) instead")]
		CGSize StringSize (UIFont font);
		
		[Bind ("sizeWithFont:forWidth:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.   Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext) instead.")]
		CGSize StringSize (UIFont font, nfloat forWidth, UILineBreakMode breakMode);
		
		[Bind ("sizeWithFont:constrainedToSize:")]
		//[Obsolete ("Deprecated in iOS7.   Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext) instead.")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize);
		
		[Bind ("sizeWithFont:constrainedToSize:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.   Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext) instead.")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode);

		[Bind ("sizeWithFont:minFontSize:actualFontSize:forWidth:lineBreakMode:")]
		// Wait for guidance here: https://devforums.apple.com/thread/203655
		//[Obsolete ("Deprecated on iOS7.   No guidance.")]
		CGSize StringSize (UIFont font, nfloat minFontSize, ref nfloat actualFontSize, nfloat forWidth, UILineBreakMode lineBreakMode);

		[Bind ("drawAtPoint:withFont:")]
		//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGPoint, UIStringAttributes) instead")]
		CGSize DrawString (CGPoint point, UIFont font);

		[Bind ("drawAtPoint:forWidth:withFont:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, UILineBreakMode breakMode);

		[Bind ("drawAtPoint:forWidth:withFont:fontSize:lineBreakMode:baselineAdjustment:")]
		//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		[Bind ("drawAtPoint:forWidth:withFont:minFontSize:actualFontSize:lineBreakMode:baselineAdjustment:")]
		//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		[Bind ("drawInRect:withFont:")]
		//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		CGSize DrawString (CGRect rect, UIFont font);

		[Bind ("drawInRect:withFont:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode);

		[Bind ("drawInRect:withFont:lineBreakMode:alignment:")]
		//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment);
#endif
#endif

#if XAMCORE_2_0
		[Internal]
#endif
		[Export ("characterAtIndex:")]
		char _characterAtIndex (nint index);

		[Export ("length")]
		nint Length {get;}

		[Export ("isEqualToString:"), Internal]
		bool IsEqualTo (IntPtr handle);
		
		[Export ("compare:")]
		NSComparisonResult Compare (NSString aString);

		[Export ("compare:options:")]
		NSComparisonResult Compare (NSString aString, NSStringCompareOptions mask);

		[Export ("compare:options:range:")]
		NSComparisonResult Compare (NSString aString, NSStringCompareOptions mask, NSRange range);

		[Export ("compare:options:range:locale:")]
		NSComparisonResult Compare (NSString aString, NSStringCompareOptions mask, NSRange range, [NullAllowed] NSLocale locale);
		
		[Export ("stringByReplacingCharactersInRange:withString:")]
		NSString Replace (NSRange range, NSString replacement);

		[Export ("commonPrefixWithString:options:")]
		NSString CommonPrefix (NSString aString, NSStringCompareOptions options);
		
		// start methods from NSStringPathExtensions category

		[Static]
		[Export("pathWithComponents:")]
		string[] PathWithComponents( string[] components );

		[Export("pathComponents")]
		string[] PathComponents { get; }

		[Export("isAbsolutePath")]
		bool IsAbsolutePath { get; }

		[Export("lastPathComponent")]
		NSString LastPathComponent { get; }

		[Export("stringByDeletingLastPathComponent")]
		NSString DeleteLastPathComponent();
 
 		[Export("stringByAppendingPathComponent:")]
 		NSString AppendPathComponent( NSString str );

 		[Export("pathExtension")]
 		NSString PathExtension { get; }

 		[Export("stringByDeletingPathExtension")]
 		NSString DeletePathExtension();

 		[Export("stringByAppendingPathExtension:")]
 		NSString AppendPathExtension( NSString str );
 
 		[Export("stringByAbbreviatingWithTildeInPath")]
 		NSString AbbreviateTildeInPath();

 		[Export("stringByExpandingTildeInPath")]
 		NSString ExpandTildeInPath();
 
 		[Export("stringByStandardizingPath")]
 		NSString StandarizePath();

 		[Export("stringByResolvingSymlinksInPath")]
 		NSString ResolveSymlinksInPath();

		[Export("stringsByAppendingPaths:")]
 		string[] AppendPaths( string[] paths );

		// end methods from NSStringPathExtensions category

		[Since (6,0)]
		[Export ("capitalizedStringWithLocale:")]
		string Capitalize ([NullAllowed] NSLocale locale);
		
		[Since (6,0)]
		[Export ("lowercaseStringWithLocale:")]
		string ToLower (NSLocale locale);
		
		[Since (6,0)]
		[Export ("uppercaseStringWithLocale:")]
		string ToUpper (NSLocale locale);

		[iOS (8,0)]
		[Export ("containsString:")]
		bool Contains (NSString str);

		[iOS (8,0), Mac (10,10)]
		[Export ("localizedCaseInsensitiveContainsString:")]
		bool LocalizedCaseInsensitiveContains (NSString str);

		[iOS (8,0), Mac (10,10)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static, Export ("stringEncodingForData:encodingOptions:convertedString:usedLossyConversion:")]
		nuint DetectStringEncoding (NSData rawData, NSDictionary options, out string convertedString, out bool usedLossyConversion);

		[iOS (8,0), Mac(10,10)]
		[Static, Wrap ("DetectStringEncoding(rawData,options == null ? null : options.Dictionary, out convertedString, out usedLossyConversion)")]
		nuint DetectStringEncoding (NSData rawData, EncodingDetectionOptions options, out string convertedString, out bool usedLossyConversion);

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionSuggestedEncodingsKey")]
		NSString EncodingDetectionSuggestedEncodingsKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionDisallowedEncodingsKey")]
		NSString EncodingDetectionDisallowedEncodingsKey { get; }
		
		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionUseOnlySuggestedEncodingsKey")]
		NSString EncodingDetectionUseOnlySuggestedEncodingsKey { get; }
		
		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionAllowLossyKey")]
		NSString EncodingDetectionAllowLossyKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionFromWindowsKey")]
		NSString EncodingDetectionFromWindowsKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionLossySubstitutionKey")]
		NSString EncodingDetectionLossySubstitutionKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionLikelyLanguageKey")]
		NSString EncodingDetectionLikelyLanguageKey { get; }

		[Export ("lineRangeForRange:")]
		NSRange LineRangeForRange (NSRange range);

		[Export ("getLineStart:end:contentsEnd:forRange:")]
		void GetLineStart (out nuint startPtr, out nuint lineEndPtr, out nuint contentsEndPtr, NSRange range);

		[iOS (9,0), Mac(10,11)]
		[Export ("variantFittingPresentationWidth:")]
		NSString GetVariantFittingPresentationWidth (nint width);

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedStandardContainsString:")]
		bool LocalizedStandardContainsString (NSString str);

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedStandardRangeOfString:")]
		NSRange LocalizedStandardRangeOfString (NSString str);

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedUppercaseString")]
		NSString LocalizedUppercaseString { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedLowercaseString")]
		NSString LocalizedLowercaseString { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedCapitalizedString")]
		NSString LocalizedCapitalizedString { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("stringByApplyingTransform:reverse:")]
		[return: NullAllowed]
		NSString TransliterateString (NSString transform, bool reverse);

		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToKatakana"), Internal]
		NSString NSStringTransformLatinToKatakana { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToHiragana"), Internal]
		NSString NSStringTransformLatinToHiragana { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToHangul"), Internal]
		NSString NSStringTransformLatinToHangul { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToArabic"), Internal]
		NSString NSStringTransformLatinToArabic { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToHebrew"), Internal]
		NSString NSStringTransformLatinToHebrew { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToThai"), Internal]
		NSString NSStringTransformLatinToThai { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToCyrillic"), Internal]
		NSString NSStringTransformLatinToCyrillic { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformLatinToGreek"), Internal]
		NSString NSStringTransformLatinToGreek { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformToLatin"), Internal]
		NSString NSStringTransformToLatin { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformMandarinToLatin"), Internal]
		NSString NSStringTransformMandarinToLatin { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformHiraganaToKatakana"), Internal]
		NSString NSStringTransformHiraganaToKatakana { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformFullwidthToHalfwidth"), Internal]
		NSString NSStringTransformFullwidthToHalfwidth { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformToXMLHex"), Internal]
		NSString NSStringTransformToXMLHex { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformToUnicodeName"), Internal]
		NSString NSStringTransformToUnicodeName { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformStripCombiningMarks"), Internal]
		NSString NSStringTransformStripCombiningMarks { get; }
		
		[iOS(9,0), Mac(10,11)]
		[Field ("NSStringTransformStripDiacritics"), Internal]
		NSString NSStringTransformStripDiacritics { get; }

		[Export ("hasPrefix:")]
		bool HasPrefix (NSString prefix);

		[Export ("hasSuffix:")]
		bool HasSuffix (NSString suffix);
	}

	[StrongDictionary ("NSString")]
	public interface EncodingDetectionOptions {
		NSStringEncoding [] EncodingDetectionSuggestedEncodings { get; set; }
		NSStringEncoding [] EncodingDetectionDisallowedEncodings { get; set; }
		bool EncodingDetectionUseOnlySuggestedEncodings { get; set; }
		bool EncodingDetectionAllowLossy { get; set; }
		bool EncodingDetectionFromWindows { get; set; }
		NSString EncodingDetectionLossySubstitution { get; set; }
		NSString EncodingDetectionLikelyLanguage { get; set; }
	}

	[BaseType (typeof (NSString))]
	// hack: it seems that generator.cs can't track NSCoding correctly ? maybe because the type is named NSString2 at that time
	public interface NSMutableString : NSCoding {
		[Export ("initWithCapacity:")]
		IntPtr Constructor (nint capacity);

		[PreSnippet ("Check (index);")]
		[Export ("insertString:atIndex:")]
		void Insert (NSString str, nint index);

		[PreSnippet ("Check (range);")]
		[Export ("deleteCharactersInRange:")]
		void DeleteCharacters (NSRange range);

		[Export ("appendString:")]
		void Append (NSString str);

		[Export ("setString:")]
		void SetString (NSString str);

		[PreSnippet ("Check (range);")]
		[Export ("replaceOccurrencesOfString:withString:options:range:")]
		nuint ReplaceOcurrences (NSString target, NSString replacement, NSStringCompareOptions options, NSRange range);

		[iOS (9,0), Mac(10,11)]
		[Export ("applyTransform:reverse:range:updatedRange:")]
		bool ApplyTransform (NSString transform, bool reverse, NSRange range, out NSRange resultingRange);

		[Export ("replaceCharactersInRange:withString:")]
		void ReplaceCharactersInRange (NSRange range, NSString aString);
	}
	
	[Category, BaseType (typeof (NSString))]
#if XAMCORE_2_0
	public partial interface NSUrlUtilities_NSString {
#else
	public partial interface NSURLUtilities_NSString {
#endif
		[Since (7,0)]
		[Export ("stringByAddingPercentEncodingWithAllowedCharacters:")]
		NSString CreateStringByAddingPercentEncoding (NSCharacterSet allowedCharacters);
	
		[Since (7,0)]
		[Export ("stringByRemovingPercentEncoding")]
		NSString CreateStringByRemovingPercentEncoding ();
	
		[Export ("stringByAddingPercentEscapesUsingEncoding:")]
		NSString CreateStringByAddingPercentEscapes (NSStringEncoding enc);
	
		[Export ("stringByReplacingPercentEscapesUsingEncoding:")]
		NSString CreateStringByReplacingPercentEscapes (NSStringEncoding enc);
	}

	
#if !MONOMAC
	// This comes from UIKit.framework/Headers/NSStringDrawing.h
	[Since (6,0)]
	[BaseType (typeof (NSObject))]
	public interface NSStringDrawingContext {
		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0)]
		[Export ("minimumTrackingAdjustment")]
		nfloat MinimumTrackingAdjustment { get; set;  }

		[Export ("actualScaleFactor")]
		nfloat ActualScaleFactor { get;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0)]
		[Export ("actualTrackingAdjustment")]
		nfloat ActualTrackingAdjustment { get;  }

		[Export ("totalBounds")]
		CGRect TotalBounds { get;  }
	}
#endif

	[BaseType (typeof (NSStream))]
	[DefaultCtorVisibility (Visibility.Protected)]
	public interface NSInputStream {
		[Export ("hasBytesAvailable")]
		bool HasBytesAvailable ();
	
		[Export ("initWithFileAtPath:")]
		IntPtr Constructor (string path);

		[DesignatedInitializer]
		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);

		[DesignatedInitializer]
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[Static]
		[Export ("inputStreamWithData:")]
		NSInputStream FromData (NSData data);
	
		[Static]
		[Export ("inputStreamWithFileAtPath:")]
		NSInputStream FromFile (string  path);

		[Static]
		[Export ("inputStreamWithURL:")]
		NSInputStream FromUrl (NSUrl url);
	}

	//
	// We expose NSString versions of these methods because it could
	// avoid an extra lookup in cases where there is a large volume of
	// calls being made and the keys are mostly tokens
	//
	[BaseType (typeof (NSObject)), Bind ("NSObject")]
	public interface NSObject2 : NSObjectProtocol {

		// those are to please the compiler while creating the definition .dll
		// but, for the final binary, we'll be using manually bounds alternatives
		// not the generated code
#pragma warning disable 108
		[Manual]
		[Export ("conformsToProtocol:")]
		bool ConformsToProtocol (IntPtr /* Protocol */ aProtocol);

		[Manual]
		[Export ("retain")]
		NSObject DangerousRetain ();

		[Manual]
		[Export ("release")]
		void DangerousRelease ();

		[Manual]
		[Export ("autorelease")]
		NSObject DangerousAutorelease ();
#pragma warning restore 108

		[Export ("doesNotRecognizeSelector:")]
		void DoesNotRecognizeSelector (Selector sel);

		[Export ("observeValueForKeyPath:ofObject:change:context:")]
		void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context);

		[Export ("addObserver:forKeyPath:options:context:")]
		void AddObserver (NSObject observer, NSString keyPath, NSKeyValueObservingOptions options, IntPtr context);

		[Wrap ("AddObserver (observer, (NSString) keyPath, options, context)")]
		void AddObserver (NSObject observer, string keyPath, NSKeyValueObservingOptions options, IntPtr context);
		
		[Export ("removeObserver:forKeyPath:context:")]
		void RemoveObserver (NSObject observer, NSString keyPath, IntPtr context);

		[Wrap ("RemoveObserver (observer, (NSString) keyPath, context)")]
		void RemoveObserver (NSObject observer, string keyPath, IntPtr context);
		
		[Export ("removeObserver:forKeyPath:")]
		void RemoveObserver (NSObject observer, NSString keyPath);

		[Wrap ("RemoveObserver (observer, (NSString) keyPath)")]
		void RemoveObserver (NSObject observer, string keyPath);

		[Export ("willChangeValueForKey:")]
		void WillChangeValue (string forKey);

		[Export ("didChangeValueForKey:")]
		void DidChangeValue (string forKey);

		[Export ("willChange:valuesAtIndexes:forKey:")]
		void WillChange (NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey);

		[Export ("didChange:valuesAtIndexes:forKey:")]
		void DidChange (NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey);

		[Export ("willChangeValueForKey:withSetMutation:usingObjects:")]
		void WillChange (NSString forKey, NSKeyValueSetMutationKind mutationKind, NSSet objects);

		[Export ("didChangeValueForKey:withSetMutation:usingObjects:")]
		void DidChange (NSString forKey, NSKeyValueSetMutationKind mutationKind, NSSet objects);

		[Static, Export ("keyPathsForValuesAffectingValueForKey:")]
		NSSet GetKeyPathsForValuesAffecting (NSString key);

		[Static, Export ("automaticallyNotifiesObserversForKey:")]
		bool AutomaticallyNotifiesObserversForKey (string key);

		[Export ("valueForKey:")]
		[MarshalNativeExceptions]
		NSObject ValueForKey (NSString key);

		[Export ("setValue:forKey:")]
		void SetValueForKey (NSObject value, NSString key);

		[Export ("valueForKeyPath:")]
		NSObject ValueForKeyPath (NSString keyPath);

		[Export ("setValue:forKeyPath:")]
		void SetValueForKeyPath (NSObject value, NSString keyPath);

		[Export ("valueForUndefinedKey:")]
		NSObject ValueForUndefinedKey (NSString key);

		[Export ("setValue:forUndefinedKey:")]
		void SetValueForUndefinedKey (NSObject value, NSString undefinedKey);

		[Export ("setNilValueForKey:")]
		void SetNilValueForKey (NSString key);

		[Export ("dictionaryWithValuesForKeys:")]
		NSDictionary GetDictionaryOfValuesFromKeys (NSString [] keys);

		[Export ("setValuesForKeysWithDictionary:")]
		void SetValuesForKeysWithDictionary (NSDictionary keyedValues);
		
		[Field ("NSKeyValueChangeKindKey")]
		NSString ChangeKindKey { get; }

		[Field ("NSKeyValueChangeNewKey")]
		NSString ChangeNewKey { get; }

		[Field ("NSKeyValueChangeOldKey")]
		NSString ChangeOldKey { get; }

		[Field ("NSKeyValueChangeIndexesKey")]
		NSString ChangeIndexesKey { get; }

		[Field ("NSKeyValueChangeNotificationIsPriorKey")]
		NSString ChangeNotificationIsPriorKey { get; }
#if MONOMAC
		// Cocoa Bindings added by Kenneth J. Pouncey 2010/11/17
		[Export ("exposedBindings")]
		NSString[] ExposedBindings ();

		[Export ("valueClassForBinding:")]
		Class BindingValueClass (string binding);

		[Export ("bind:toObject:withKeyPath:options:")]
		void Bind (string binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options);

		[Export ("unbind:")]
		void Unbind (string binding);

		[Export ("infoForBinding:")]
		NSDictionary BindingInfo (string binding);

		[Export ("optionDescriptionsForBinding:")]
		NSObject[] BindingOptionDescriptions (string aBinding);

		// NSPlaceholders (informal) protocol
		[Static]
		[Export ("defaultPlaceholderForMarker:withBinding:")]
		NSObject GetDefaultPlaceholder (NSObject marker, string binding);

		[Static]
		[Export ("setDefaultPlaceholder:forMarker:withBinding:")]
		void SetDefaultPlaceholder (NSObject placeholder, NSObject marker, string binding);

		[Export ("objectDidEndEditing:")]
		void ObjectDidEndEditing (NSObject editor);

		[Export ("commitEditing")]
		bool CommitEditing ();

		[Export ("commitEditingWithDelegate:didCommitSelector:contextInfo:")]
		//void CommitEditingWithDelegateDidCommitSelectorContextInfo (NSObject objDelegate, Selector didCommitSelector, IntPtr contextInfo);
		void CommitEditing (NSObject objDelegate, Selector didCommitSelector, IntPtr contextInfo);
#endif
		[Export ("methodForSelector:")]
		IntPtr GetMethodForSelector (Selector sel);

		[PreSnippet ("if (!(this is INSCopying)) throw new InvalidOperationException (\"Type does not conform to NSCopying\");")]
		[Export ("copy")]
		[return: Release ()]
		NSObject Copy ();

		[PreSnippet ("if (!(this is INSMutableCopying)) throw new InvalidOperationException (\"Type does not conform to NSMutableCopying\");")]
		[Export ("mutableCopy")]
		[return: Release ()]
		NSObject MutableCopy ();

		//
		// Extra Perform methods, with selectors
		//
		[Export ("performSelector:withObject:afterDelay:inModes:")]
		void PerformSelector (Selector selector, [NullAllowed] NSObject withObject, double afterDelay, NSString [] nsRunLoopModes);

		[Export ("performSelector:withObject:afterDelay:")]
		void PerformSelector (Selector selector, [NullAllowed] NSObject withObject, double delay);
		
		[Export ("performSelector:onThread:withObject:waitUntilDone:")]
		void PerformSelector (Selector selector, NSThread onThread, [NullAllowed] NSObject withObject, bool waitUntilDone);
		
		[Export ("performSelector:onThread:withObject:waitUntilDone:modes:")]
		void PerformSelector (Selector selector, NSThread onThread, [NullAllowed] NSObject withObject, bool waitUntilDone, NSString [] nsRunLoopModes);
		
		[Static, Export ("cancelPreviousPerformRequestsWithTarget:")]
		void CancelPreviousPerformRequest (NSObject aTarget);

		[Static, Export ("cancelPreviousPerformRequestsWithTarget:selector:object:")]
		void CancelPreviousPerformRequest (NSObject aTarget, Selector selector, [NullAllowed] NSObject argument);

		[iOS (8,0), Mac (10,10)]
		[NoWatch]
		[Export ("prepareForInterfaceBuilder")]
		void PrepareForInterfaceBuilder ();

		[NoWatch]
		[Export ("awakeFromNib")]
		void AwakeFromNib ();
	}

	[Protocol (Name = "NSObject")] // exists both as a type and a protocol in ObjC, Swift uses NSObjectProtocol
	public interface NSObjectProtocol {

		[Abstract]
		[Export ("description")]
		string Description { get; }

		[Export ("debugDescription")]
		string DebugDescription { get; }

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("superclass")]
		Class Superclass { get; }

		// defined multiple times (method, property and even static), one (not static) is required
		// and that match Apple's documentation
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("hash")]
		nuint GetNativeHash ();

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSObject anObject);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("class")]
		Class Class { get; }

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Export ("self")][Transient]
		NSObject Self { get; }

		[Abstract]
		[Export ("performSelector:")]
		NSObject PerformSelector (Selector aSelector);

		[Abstract]
		[Export ("performSelector:withObject:")]
		NSObject PerformSelector (Selector aSelector, [NullAllowed] NSObject anObject);

		[Abstract]
		[Export ("performSelector:withObject:withObject:")]
		NSObject PerformSelector (Selector aSelector, [NullAllowed] NSObject object1, [NullAllowed] NSObject object2);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isProxy")]
		bool IsProxy { get; }

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isKindOfClass:")]
		bool IsKindOfClass ([NullAllowed] Class aClass);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isMemberOfClass:")]
		bool IsMemberOfClass ([NullAllowed] Class aClass);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("conformsToProtocol:")]
		bool ConformsToProtocol ([NullAllowed] IntPtr /* Protocol */ aProtocol);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("respondsToSelector:")]
		bool RespondsToSelector ([NullAllowed] Selector sel);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("retain")]
		NSObject DangerousRetain ();

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("release")]
		void DangerousRelease ();

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("autorelease")]
		NSObject DangerousAutorelease ();

		[Abstract]
		[Export ("retainCount")]
#if XAMCORE_2_0
		nuint RetainCount { get; }
#else
		nint RetainCount { get; }
#endif

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("zone")]
		NSZone Zone { get; }
	}

	[BaseType (typeof (NSObject))]
	[Since (4,0)]
	public interface NSOperation {
		[Export ("start")]
		void Start ();

		[Export ("main")]
		void Main ();

		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("isExecuting")]
		bool IsExecuting { get; }

		[Export ("isFinished")]
		bool IsFinished { get; }

		[Export ("isConcurrent")]
		bool IsConcurrent { get; }

		[Export ("isReady")]
		bool IsReady { get; }

		[Export ("addDependency:")][PostGet ("Dependencies")]
		void AddDependency (NSOperation op);

		[Export ("removeDependency:")][PostGet ("Dependencies")]
		void RemoveDependency (NSOperation op);

		[Export ("dependencies")]
		NSOperation [] Dependencies { get; }

		[NullAllowed]
		[Export ("completionBlock", ArgumentSemantic.Copy)]
		Action CompletionBlock { get; set; }

		[Export ("waitUntilFinished")]
		void WaitUntilFinished ();

		[Export ("threadPriority")]
		double ThreadPriority { get; set; }

		//Detected properties
		[Export ("queuePriority")]
		NSOperationQueuePriority QueuePriority { get; set; }

		[iOS (7,0)]
		[Export ("asynchronous")]
		bool Asynchronous { [Bind ("isAsynchronous")] get; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		[iOS (8,0)][Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }
	}

	[BaseType (typeof (NSOperation))]
	[Since (4,0)]
	public interface NSBlockOperation {
		[Static]
		[Export ("blockOperationWithBlock:")]
		NSBlockOperation Create (/* non null */ NSAction method);

		[Export ("addExecutionBlock:")]
		void AddExecutionBlock (/* non null */ NSAction method);

		[Export ("executionBlocks")]
		NSObject [] ExecutionBlocks { get; }
	}

	[BaseType (typeof (NSObject))]
	[Since (4,0)]
	public interface NSOperationQueue {
		[Export ("addOperation:")][PostGet ("Operations")]
		void AddOperation ([NullAllowed] NSOperation op);

		[Export ("addOperations:waitUntilFinished:")][PostGet ("Operations")]
		void AddOperations ([NullAllowed] NSOperation [] operations, bool waitUntilFinished);

		[Export ("addOperationWithBlock:")][PostGet ("Operations")]
		void AddOperation (/* non null */ NSAction operation);

		[Export ("operations")]
		NSOperation [] Operations { get; }

		[Export ("operationCount")]
		nint OperationCount { get; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("cancelAllOperations")][PostGet ("Operations")]
		void CancelAllOperations ();

		[Export ("waitUntilAllOperationsAreFinished")]
		void WaitUntilAllOperationsAreFinished ();

		[Static]
		[Export ("currentQueue")]
		NSOperationQueue CurrentQueue { get; }

		[Static]
		[Export ("mainQueue")]
		NSOperationQueue MainQueue { get; }

		//Detected properties
		[Export ("maxConcurrentOperationCount")]
		nint MaxConcurrentOperationCount { get; set; }

		[Export ("suspended")]
		bool Suspended { [Bind ("isSuspended")]get; set; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		[NullAllowed]
		[iOS (8,0)][Mac (10,10)]
		[Export ("underlyingQueue", ArgumentSemantic.UnsafeUnretained)]
		DispatchQueue UnderlyingQueue { get; set; }
		
	}

#if XAMCORE_2_0
	public interface NSOrderedSet<TKey> : NSOrderedSet {}
#endif

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	public interface NSOrderedSet : NSSecureCoding, NSMutableCopying {
		[Export ("initWithObject:")]
		IntPtr Constructor (NSObject start);

		[Export ("initWithArray:"), Internal]
		IntPtr Constructor (NSArray array);

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet source);

		[Export ("initWithOrderedSet:")]
		IntPtr Constructor (NSOrderedSet source);

		[Export ("count")]
		nint Count { get; }

		[Internal]
		[Sealed]
		[Export ("objectAtIndex:")]
		IntPtr _GetObject (nint idx);

		[Export ("objectAtIndex:"), Internal]
		NSObject GetObject (nint idx);

		[Export ("array"), Internal]
		IntPtr _ToArray ();

		[Internal]
		[Sealed]
		[Export ("indexOfObject:")]
		nint _IndexOf (IntPtr obj);

		[Export ("indexOfObject:")]
		nint IndexOf (NSObject obj);

		[Export ("objectEnumerator"), Internal]
		NSEnumerator _GetEnumerator ();

		[Internal]
		[Sealed]
		[Export ("set")]
		IntPtr _AsSet ();

		[Export ("set")]
		NSSet AsSet ();

		[Internal]
		[Sealed]
		[Export ("containsObject:")]
		bool _Contains (IntPtr obj);

		[Export ("containsObject:")]
		bool Contains (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("firstObject")]
		IntPtr _FirstObject ();

		[Export ("firstObject")]
		NSObject FirstObject ();

		[Internal]
		[Sealed]
		[Export ("lastObject")]
		IntPtr _LastObject ();

		[Export ("lastObject")]
		NSObject LastObject ();

		[Export ("isEqualToOrderedSet:")]
		bool IsEqualToOrderedSet (NSOrderedSet other);

		[Export ("intersectsOrderedSet:")]
		bool Intersects (NSOrderedSet other);

		[Export ("intersectsSet:")]
		bool Intersects (NSSet other);

		[Export ("isSubsetOfOrderedSet:")]
		bool IsSubset (NSOrderedSet other);

		[Export ("isSubsetOfSet:")]
		bool IsSubset (NSSet other);

		[Export ("reversedOrderedSet")]
		NSOrderedSet GetReverseOrderedSet ();
	}

#if XAMCORE_2_0
	public interface NSMutableOrderedSet<TKey> : NSMutableOrderedSet {}
#endif

	[Since (5,0)]
	[BaseType (typeof (NSOrderedSet))]
	public interface NSMutableOrderedSet {
		[Export ("initWithObject:")]
		IntPtr Constructor (NSObject start);

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet source);

		[Export ("initWithOrderedSet:")]
		IntPtr Constructor (NSOrderedSet source);

		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		IntPtr Constructor (nint capacity);

		[Export ("initWithArray:"), Internal]
		IntPtr Constructor (NSArray array);

		[Export ("unionSet:"), Internal]
		void UnionSet (NSSet other);

		[Export ("minusSet:"), Internal]
		void MinusSet (NSSet other);

		[Export ("unionOrderedSet:"), Internal]
		void UnionSet (NSOrderedSet other);

		[Export ("minusOrderedSet:"), Internal]
		void MinusSet (NSOrderedSet other);

		[Internal]
		[Sealed]
		[Export ("insertObject:atIndex:")]
		void _Insert (IntPtr obj, nint atIndex);

		[Export ("insertObject:atIndex:")]
		void Insert (NSObject obj, nint atIndex);

		[Export ("removeObjectAtIndex:")]
		void Remove (nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectAtIndex:withObject:")]
		void _Replace (nint objectAtIndex, IntPtr newObject);

		[Export ("replaceObjectAtIndex:withObject:")]
		void Replace (nint objectAtIndex, NSObject newObject);

		[Internal]
		[Sealed]
		[Export ("addObject:")]
		void _Add (IntPtr obj);

		[Export ("addObject:")]
		void Add (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("addObjectsFromArray:")]
		void _AddObjects (NSArray source);

		[Export ("addObjectsFromArray:")]
		void AddObjects (NSObject [] source);

		[Internal]
		[Sealed]
		[Export ("insertObjects:atIndexes:")]
		void _InsertObjects (NSArray objects, NSIndexSet atIndexes);

		[Export ("insertObjects:atIndexes:")]
		void InsertObjects (NSObject [] objects, NSIndexSet atIndexes);

		[Export ("removeObjectsAtIndexes:")]
		void RemoveObjects (NSIndexSet indexSet);

		[Export ("exchangeObjectAtIndex:withObjectAtIndex:")]
		void ExchangeObject (nint first, nint second);

		[Export ("moveObjectsAtIndexes:toIndex:")]
		void MoveObjects (NSIndexSet indexSet, nint destination);

		[Internal]
		[Sealed]
		[Export ("setObject:atIndex:")]
		void _SetObject (IntPtr obj, nint index);

		[Export ("setObject:atIndex:")]
		void SetObject (NSObject obj, nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectsAtIndexes:withObjects:")]
		void _ReplaceObjects (NSIndexSet indexSet, NSArray replacementObjects);

		[Export ("replaceObjectsAtIndexes:withObjects:")]
		void ReplaceObjects (NSIndexSet indexSet, NSObject [] replacementObjects);

		[Export ("removeObjectsInRange:")]
		void RemoveObjects (NSRange range);

		[Export ("removeAllObjects")]
		void RemoveAllObjects ();

		[Internal]
		[Sealed]
		[Export ("removeObject:")]
		void _RemoveObject (IntPtr obj);

		[Export ("removeObject:")]
		void RemoveObject (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("removeObjectsInArray:")]
		void _RemoveObjects (NSArray objects);

		[Export ("removeObjectsInArray:")]
		void RemoveObjects (NSObject [] objects);

		[Export ("intersectOrderedSet:")]
		void Intersect (NSOrderedSet intersectWith);

		[Export ("intersectSet:")]
		void Intersect (NSSet intersectWith);

		[Export ("sortUsingComparator:")]
		void Sort (NSComparator comparator);

		[Export ("sortWithOptions:usingComparator:")]
		void Sort (NSSortOptions sortOptions, NSComparator comparator);

		[Export ("sortRange:options:usingComparator:")]
		void SortRange (NSRange range, NSSortOptions sortOptions, NSComparator comparator);
	}
	
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[__NSArrayM insertObject:atIndex:]: object cannot be nil
	[DisableDefaultCtor]
	public interface NSOrthography : NSSecureCoding, NSCopying {
		[Export ("dominantScript")]
		string DominantScript { get;  }

		[Export ("languageMap")]
		NSDictionary LanguageMap { get;  }

		[Export ("dominantLanguage")]
		string DominantLanguage { get;  }

		[Export ("allScripts")]
		string [] AllScripts { get;  }

		[Export ("allLanguages")]
		string [] AllLanguages { get;  }

		[Export ("languagesForScript:")]
		string [] LanguagesForScript (string script);

		[Export ("dominantLanguageForScript:")]
		string DominantLanguageForScript (string script);

		[DesignatedInitializer]
		[Export ("initWithDominantScript:languageMap:")]
		IntPtr Constructor (string dominantScript, [NullAllowed] NSDictionary languageMap);
	}
	
	[BaseType (typeof (NSStream))]
	[DisableDefaultCtor] // crash when used
	public interface NSOutputStream {
		[Export ("initToMemory")]
		IntPtr Constructor ();

		[Export ("hasSpaceAvailable")]
		bool HasSpaceAvailable ();
	
		//[Export ("initToBuffer:capacity:")]
		//IntPtr Constructor (uint8_t  buffer, NSUInteger capacity);

		[Export ("initToFileAtPath:append:")]
		IntPtr Constructor (string path, bool shouldAppend);

		[Static]
		[Export ("outputStreamToMemory")]
#if XAMCORE_2_0
		NSObject OutputStreamToMemory ();
#else
		NSOutputStream OutputStreamToMemory ();
#endif

		//[Static]
		//[Export ("outputStreamToBuffer:capacity:")]
		//NSObject OutputStreamToBuffer (uint8_t  buffer, NSUInteger capacity);

		[Static]
		[Export ("outputStreamToFileAtPath:append:")]
		NSOutputStream CreateFile (string path, bool shouldAppend);
	}

	[BaseType (typeof (NSObject), Name="NSHTTPCookie")]
	// default 'init' crash both simulator and devices
	[DisableDefaultCtor]
	public interface NSHttpCookie {
		[Export ("initWithProperties:")]
		IntPtr Constructor (NSDictionary properties);

		[Export ("cookieWithProperties:"), Static]
		NSHttpCookie CookieFromProperties (NSDictionary properties);

		[Export ("requestHeaderFieldsWithCookies:"), Static]
		NSDictionary RequestHeaderFieldsWithCookies (NSHttpCookie [] cookies);

		[Export ("cookiesWithResponseHeaderFields:forURL:"), Static]
		NSHttpCookie [] CookiesWithResponseHeaderFields (NSDictionary headerFields, NSUrl url);

		[Export ("properties")]
		NSDictionary Properties { get; }

		[Export ("version")]
		nuint Version { get; }

		[Export ("value")]
		string Value { get; }

		[Export ("expiresDate")]
		NSDate ExpiresDate { get; }

		[Export ("isSessionOnly")]
		bool IsSessionOnly { get; }

		[Export ("domain")]
		string Domain { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("path")]
		string Path { get; }

		[Export ("isSecure")]
		bool IsSecure { get; }

		[Export ("isHTTPOnly")]
		bool IsHttpOnly { get; }

		[Export ("comment")]
		string Comment { get; }

		[Export ("commentURL")]
		NSUrl CommentUrl { get; }

		[Export ("portList")]
		NSNumber [] PortList { get; }

#if XAMCORE_2_0
		[Field ("NSHTTPCookieName")]
		NSString KeyName { get; }

		[Field ("NSHTTPCookieValue")]
		NSString KeyValue { get; }

		[Field ("NSHTTPCookieOriginURL")]
		NSString KeyOriginUrl { get; }

		[Field ("NSHTTPCookieVersion")]
		NSString KeyVersion { get; }

		[Field ("NSHTTPCookieDomain")]
		NSString KeyDomain { get; }

		[Field ("NSHTTPCookiePath")]
		NSString KeyPath { get; }

		[Field ("NSHTTPCookieSecure")]
		NSString KeySecure { get; }

		[Field ("NSHTTPCookieExpires")]
		NSString KeyExpires { get; }

		[Field ("NSHTTPCookieComment")]
		NSString KeyComment { get; }

		[Field ("NSHTTPCookieCommentURL")]
		NSString KeyCommentUrl { get; }

		[Field ("NSHTTPCookieDiscard")]
		NSString KeyDiscard { get; }

		[Field ("NSHTTPCookieMaximumAge")]
		NSString KeyMaximumAge { get; }

		[Field ("NSHTTPCookiePort")]
		NSString KeyPort { get; }
#endif
	}

	[BaseType (typeof (NSObject), Name="NSHTTPCookieStorage")]
	// NSHTTPCookieStorage implements a singleton object -> use SharedStorage since 'init' returns NIL
	[DisableDefaultCtor]
	public interface NSHttpCookieStorage {
		[Export ("sharedHTTPCookieStorage"), Static]
		NSHttpCookieStorage SharedStorage { get; }

		[Export ("cookies")]
		NSHttpCookie [] Cookies { get; }

		[Export ("setCookie:")]
		void SetCookie (NSHttpCookie cookie);

		[Export ("deleteCookie:")]
		void DeleteCookie (NSHttpCookie cookie);

		[Export ("cookiesForURL:")]
		NSHttpCookie [] CookiesForUrl (NSUrl url);

		[Export ("setCookies:forURL:mainDocumentURL:")]
		void SetCookies (NSHttpCookie [] cookies, NSUrl forUrl, NSUrl mainDocumentUrl);

		[Export ("cookieAcceptPolicy")]
		NSHttpCookieAcceptPolicy AcceptPolicy { get; set; }

		[Since (5,0)]
		[Export ("sortedCookiesUsingDescriptors:")]
		NSHttpCookie [] GetSortedCookies (NSSortDescriptor [] sortDescriptors);

		// @required - (void)removeCookiesSinceDate:(NSDate *)date;
		[Mac (10,10)][iOS (8,0)]
		[Export ("removeCookiesSinceDate:")]
		void RemoveCookiesSinceDate (NSDate date);

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("sharedCookieStorageForGroupContainerIdentifier:")]
		NSHttpCookieStorage GetSharedCookieStorage (string groupContainerIdentifier);
		
#if !MONOMAC || XAMCORE_2_0
		[Mac (10,10)][iOS (8,0)]
		[Export ("getCookiesForTask:completionHandler:")]
		void GetCookiesForTask (NSUrlSessionTask task, Action<NSHttpCookie []> completionHandler);

		[Mac (10,10)][iOS (8,0)]
		[Export ("storeCookies:forTask:")]
		void StoreCookies (NSHttpCookie [] cookies, NSUrlSessionTask task);
#endif
#if XAMCORE_2_0
		[Notification]
		[Field ("NSHTTPCookieManagerAcceptPolicyChangedNotification")]
		NSString CookiesChangedNotification { get; }

		[Notification]
		[Field ("NSHTTPCookieManagerCookiesChangedNotification")]
		NSString AcceptPolicyChangedNotification { get; }
#endif
	}
	
	[BaseType (typeof (NSUrlResponse), Name="NSHTTPURLResponse")]
	public interface NSHttpUrlResponse {
		[Export ("initWithURL:MIMEType:expectedContentLength:textEncodingName:")]
		IntPtr Constructor (NSUrl url, string mimetype, nint expectedContentLength, [NullAllowed] string textEncodingName);

		[Since (5,0)]
		[Export ("initWithURL:statusCode:HTTPVersion:headerFields:")]
		IntPtr Constructor (NSUrl url, nint statusCode, string httpVersion, NSDictionary headerFields);
		
		[Export ("statusCode")]
		nint StatusCode { get; }

		[Export ("allHeaderFields")]
		NSDictionary AllHeaderFields { get; }

		[Export ("localizedStringForStatusCode:")][Static]
		string LocalizedStringForStatusCode (nint statusCode);
	}
	
	[BaseType (typeof (NSObject))]
#if MONOMAC
	[DisableDefaultCtor] // An uncaught exception was raised: -[__NSCFDictionary removeObjectForKey:]: attempt to remove nil key
#endif
	public partial interface NSBundle {
		[Export ("mainBundle")][Static]
		NSBundle MainBundle { get; }

		[Export ("bundleWithPath:")][Static]
		NSBundle FromPath (string path);

		[DesignatedInitializer]
		[Export ("initWithPath:")]
		IntPtr Constructor (string path);

		[Export ("bundleForClass:")][Static]
		NSBundle FromClass (Class c);

		[Export ("bundleWithIdentifier:")][Static]
		NSBundle FromIdentifier (string str);

		[Export ("allBundles")][Static]
		NSBundle [] _AllBundles { get; }

		[Export ("allFrameworks")][Static]
		NSBundle [] AllFrameworks { get; }

		[Export ("load")]
		bool Load ();

		[Export ("isLoaded")]
		bool IsLoaded { get; }

		[Export ("unload")]
		bool Unload ();

		[Export ("bundlePath")]
		string BundlePath { get; }
		
		[Export ("resourcePath")]
		string  ResourcePath { get; }
		
		[Export ("executablePath")]
		string ExecutablePath { get; }
		
		[Export ("pathForAuxiliaryExecutable:")]
		string PathForAuxiliaryExecutable (string s);
		

		[Export ("privateFrameworksPath")]
		string PrivateFrameworksPath { get; }
		
		[Export ("sharedFrameworksPath")]
		string SharedFrameworksPath { get; }
		
		[Export ("sharedSupportPath")]
		string SharedSupportPath { get; }
		
		[Export ("builtInPlugInsPath")]
		string BuiltinPluginsPath { get; }
		
		[Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[Export ("classNamed:")]
		Class ClassNamed (string className);
		
		[Export ("principalClass")]
		Class PrincipalClass { get; }

		[Export ("pathForResource:ofType:inDirectory:")][Static]
		string PathForResourceAbsolute (string name, [NullAllowed] string ofType, string bundleDirectory);
		
		[Export ("pathForResource:ofType:")]
		string PathForResource (string name, [NullAllowed] string ofType);

		[Export ("pathForResource:ofType:inDirectory:")]
		string PathForResource (string name, [NullAllowed] string ofType, [NullAllowed] string subpath);
		
		[Export ("pathForResource:ofType:inDirectory:forLocalization:")]
		string PathForResource (string name, [NullAllowed] string ofType, string subpath, string localizationName);

		[Export ("localizedStringForKey:value:table:")]
		string LocalizedString ([NullAllowed] string key, [NullAllowed] string value, [NullAllowed] string table);

		[Export ("objectForInfoDictionaryKey:")]
		NSObject ObjectForInfoDictionary (string key);

		[Export ("developmentLocalization")]
		string DevelopmentLocalization { get; }
		
		[Export ("infoDictionary")]
		NSDictionary InfoDictionary{ get; }

		// Additions from AppKit
#if MONOMAC
		[Mac (10,8)]
		[Export ("loadNibNamed:owner:topLevelObjects:")]
		bool LoadNibNamed (string nibName, [NullAllowed] NSObject owner, out NSArray topLevelObjects);

		// https://developer.apple.com/library/mac/#documentation/Cocoa/Reference/ApplicationKit/Classes/NSBundle_AppKitAdditions/Reference/Reference.html
		[Static]
		[Export ("loadNibNamed:owner:")]
		bool LoadNib (string nibName, NSObject owner);

		[Export ("pathForImageResource:")]
		string PathForImageResource (string resource);

		[Export ("pathForSoundResource:")]
		string PathForSoundResource (string resource);

		[Mac (10,6)]
		[Export ("URLForImageResource:")]
		NSUrl GetUrlForImageResource (string resource);

		[Export ("contextHelpForKey:")]
		NSAttributedString GetContextHelp (string key);
#else
		// http://developer.apple.com/library/ios/#documentation/uikit/reference/NSBundle_UIKitAdditions/Introduction/Introduction.html
		[NoWatch]
		[Export ("loadNibNamed:owner:options:")]
		NSArray LoadNib (string nibName, [NullAllowed] NSObject owner, [NullAllowed] NSDictionary options);
#endif

		[Export ("bundleURL")]
		[Since (4,0)]
		NSUrl BundleUrl { get; }
		
		[Export ("resourceURL")]
		[Since (4,0)]
		NSUrl ResourceUrl { get; }

		[Export ("executableURL")]
		[Since (4,0)]
		NSUrl ExecutableUrl { get; }

		[Export ("URLForAuxiliaryExecutable:")]
		[Since (4,0)]
		NSUrl UrlForAuxiliaryExecutable (string executable);

		[Export ("privateFrameworksURL")]
		[Since (4,0)]
		NSUrl PrivateFrameworksUrl { get; }

		[Export ("sharedFrameworksURL")]
		[Since (4,0)]
		NSUrl SharedFrameworksUrl { get; }

		[Export ("sharedSupportURL")]
		[Since (4,0)]
		NSUrl SharedSupportUrl { get; }

		[Export ("builtInPlugInsURL")]
		[Since (4,0)]
		NSUrl BuiltInPluginsUrl { get; }

		[Export ("initWithURL:")]
		[Since (4,0)]
		IntPtr Constructor (NSUrl url);
		
		[Static, Export ("bundleWithURL:")]
		[Since (4,0)]
		NSBundle FromUrl (NSUrl url);

		[Export ("preferredLocalizations")]
		string [] PreferredLocalizations { get; }

		[Export ("localizations")]
		string [] Localizations { get; }

		[Since (7,0)]
		[Export ("appStoreReceiptURL")]
		NSUrl AppStoreReceiptUrl { get; }

		[Export ("pathsForResourcesOfType:inDirectory:")]
		string [] PathsForResources (string fileExtension, [NullAllowed] string subDirectory);

		[Export ("pathsForResourcesOfType:inDirectory:forLocalization:")]
		string [] PathsForResources (string fileExtension, [NullAllowed] string subDirectory, [NullAllowed] string localizationName);

		[Static, Export ("pathsForResourcesOfType:inDirectory:")]
		string [] GetPathsForResources (string fileExtension, string bundlePath);

		[Static, Export ("URLForResource:withExtension:subdirectory:inBundleWithURL:")]
		NSUrl GetUrlForResource (string name, string fileExtension, [NullAllowed] string subdirectory, NSUrl bundleURL);

		[Static, Export ("URLsForResourcesWithExtension:subdirectory:inBundleWithURL:")]
		NSUrl [] GetUrlsForResourcesWithExtension (string fileExtension, [NullAllowed] string subdirectory, NSUrl bundleURL);

		[Export ("URLForResource:withExtension:")]
		NSUrl GetUrlForResource (string name, string fileExtension);

		[Export ("URLForResource:withExtension:subdirectory:")]
		NSUrl GetUrlForResource (string name, string fileExtension, [NullAllowed] string subdirectory);

		[Export ("URLForResource:withExtension:subdirectory:localization:")]
		NSUrl GetUrlForResource (string name, string fileExtension, [NullAllowed] string subdirectory, [NullAllowed] string localizationName);

		[Export ("URLsForResourcesWithExtension:subdirectory:")]
		NSUrl [] GetUrlsForResourcesWithExtension (string fileExtension, [NullAllowed] string subdirectory);

		[Export ("URLsForResourcesWithExtension:subdirectory:localization:")]
		NSUrl [] GetUrlsForResourcesWithExtension (string fileExtension, [NullAllowed] string subdirectory,  [NullAllowed] string localizationName);

#if !MONOMAC
		[iOS (9,0)]
		[Export ("preservationPriorityForTag:")]
		double GetPreservationPriority (NSString tag);

		[iOS (9,0)]
		[Export ("setPreservationPriority:forTags:")]
		void SetPreservationPriority (double priority, NSSet<NSString> tags);
#endif
	}

#if !MONOMAC
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSBundleResourceRequest : NSProgressReporting
	{
		[Export ("initWithTags:")]
		IntPtr Constructor (NSSet<NSString> tags);
	
		[Export ("initWithTags:bundle:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSSet<NSString> tags, NSBundle bundle);
	
		[Export ("loadingPriority")]
		double LoadingPriority { get; set; }
	
		[Export ("tags", ArgumentSemantic.Copy)]
		NSSet<NSString> Tags { get; }
	
		[Export ("bundle", ArgumentSemantic.Strong)]
		NSBundle Bundle { get; }
	
		[Export ("beginAccessingResourcesWithCompletionHandler:")]
		[Async]
		void BeginAccessingResources (Action<NSError> completionHandler);
	
		[Export ("conditionallyBeginAccessingResourcesWithCompletionHandler:")]
		[Async]
		void ConditionallyBeginAccessingResources (Action<bool> completionHandler);
	
		[Export ("endAccessingResources")]
		void EndAccessingResources ();
	
		[Field ("NSBundleResourceRequestLowDiskSpaceNotification")]
		[Notification]
		NSString LowDiskSpaceNotification { get; }
		
		[Field ("NSBundleResourceRequestLoadingPriorityUrgent")]
		double LoadingPriorityUrgent { get; }
	}
#endif
		
	[BaseType (typeof (NSObject))]
	public interface NSIndexPath : NSCoding, NSSecureCoding, NSCopying {
		[Export ("indexPathWithIndex:")][Static]
		NSIndexPath FromIndex (nuint index);

		[Export ("indexPathWithIndexes:length:")][Internal][Static]
		NSIndexPath _FromIndex (IntPtr indexes, nint len);

		[Export ("indexPathByAddingIndex:")]
		NSIndexPath IndexPathByAddingIndex (nuint index);
		
		[Export ("indexPathByRemovingLastIndex")]
		NSIndexPath IndexPathByRemovingLastIndex ();

		[Export ("indexAtPosition:")]
		nuint IndexAtPosition (nint position);

		[Export ("length")]
		nint Length { get; } 

		[Export ("getIndexes:")][Internal]
		void _GetIndexes (IntPtr target);

		[Mac (10,9)][iOS (7,0)]
		[Export ("getIndexes:range:")][Internal]
		void _GetIndexes (IntPtr target, NSRange positionRange);

		[Export ("compare:")]
		nint Compare (NSIndexPath other);

#if !MONOMAC
		// NSIndexPath UIKit Additions Reference
		// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/NSIndexPath_UIKitAdditions/Reference/Reference.html

#if XAMCORE_2_0
		// see monotouch/src/UIKit/Addition.cs for int-returning Row/Section properties

		[NoWatch]
		[Export ("row")]
		nint LongRow { get; }

		[NoWatch]
		[Export ("section")]
		nint LongSection { get; }
#else
		[Export ("row")]
		nint Row { get; }

		[Export ("section")]
		nint Section { get; }
#endif

		[NoWatch]
		[Static]
		[Export ("indexPathForRow:inSection:")]
		NSIndexPath FromRowSection (nint row, nint section);
#else

		[Mac (10,11)]
		[Export ("section")]
		nint Section { get; }
#endif

		[NoWatch]
		[Static]
		[iOS (6,0)][Mac (10,11)]
		[Export ("indexPathForItem:inSection:")]
		NSIndexPath FromItemSection (nint item, nint section);

		[NoWatch]
		[Export ("item")]
		[iOS (6,0)][Mac (10,11)]
		nint Item { get; }
	}

	public delegate void NSRangeIterator (NSRange range, ref bool stop);
	
	[BaseType (typeof (NSObject))]
	public interface NSIndexSet : NSCoding, NSSecureCoding, NSMutableCopying {
		[Static, Export ("indexSetWithIndex:")]
		NSIndexSet FromIndex (nint idx);

		[Static, Export ("indexSetWithIndexesInRange:")]
		NSIndexSet FromNSRange (NSRange indexRange);
		
		[Export ("initWithIndex:")]
		IntPtr Constructor (nuint index);

		[DesignatedInitializer]
		[Export ("initWithIndexSet:")]
		IntPtr Constructor (NSIndexSet other);

		[Export ("count")]
		nint Count { get; }

		[Export ("isEqualToIndexSet:")]
		bool IsEqual (NSIndexSet other);

		[Export ("firstIndex")]
		nuint FirstIndex { get; }

		[Export ("lastIndex")]
		nuint LastIndex { get; }

		[Export ("indexGreaterThanIndex:")]
		nuint IndexGreaterThan (nuint index);

		[Export ("indexLessThanIndex:")]
		nuint IndexLessThan (nuint index);

		[Export ("indexGreaterThanOrEqualToIndex:")]
		nuint IndexGreaterThanOrEqual (nuint index);

		[Export ("indexLessThanOrEqualToIndex:")]
		nuint IndexLessThanOrEqual (nuint index);

		[Export ("containsIndex:")]
		bool Contains (nuint index);

		[Export ("containsIndexes:")]
		bool Contains (NSIndexSet indexes);

		[Since (5,0)]
		[Export ("enumerateRangesUsingBlock:")]
		void EnumerateRanges (NSRangeIterator iterator);

		[Since (5,0)]
		[Export ("enumerateRangesWithOptions:usingBlock:")]
		void EnumerateRanges (NSEnumerationOptions opts, NSRangeIterator iterator);

		[Since (5,0)]
		[Export ("enumerateRangesInRange:options:usingBlock:")]
		void EnumerateRanges (NSRange range, NSEnumerationOptions opts, NSRangeIterator iterator);

		[Export ("enumerateIndexesUsingBlock:")]
		void EnumerateIndexes (EnumerateIndexSetCallback enumeratorCallback);

		[Export ("enumerateIndexesWithOptions:usingBlock:")]
		void EnumerateIndexes (NSEnumerationOptions opts, EnumerateIndexSetCallback enumeratorCallback);

		[Export ("enumerateIndexesInRange:options:usingBlock:")]
		void EnumerateIndexes (NSRange range, NSEnumerationOptions opts, EnumerateIndexSetCallback enumeratorCallback);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // from the docs: " you should not create these objects using alloc and init."
	public interface NSInvocation {

		[Export ("selector")]
		Selector Selector { get; set; }

		[Export ("target", ArgumentSemantic.Assign), NullAllowed]
		NSObject Target { get; set; }

		// FIXME: We need some special marshaling support to handle these buffers...
		[Internal, Export ("setArgument:atIndex:")]
		void _SetArgument (IntPtr buffer, nint index);

		[Internal, Export ("getArgument:atIndex:")]
		void _GetArgument (IntPtr buffer, nint index);

		[Internal, Export ("setReturnValue:")]
		void _SetReturnValue (IntPtr buffer);

		[Internal, Export ("getReturnValue:")]
		void _GetReturnValue (IntPtr buffer);

		[Export ("invoke")]
		void Invoke ();

		[Export ("invokeWithTarget:")]
		void Invoke (NSObject target);

		[Export ("methodSignature")]
		NSMethodSignature MethodSignature { get; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	public partial interface NSItemProvider : NSCopying {
		[DesignatedInitializer]
		[Export ("initWithItem:typeIdentifier:")]
		IntPtr Constructor ([NullAllowed] NSObject item, string typeIdentifier);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl fileUrl);

		[Export ("registeredTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] RegisteredTypeIdentifiers { get; }

		[Export ("registerItemForTypeIdentifier:loadHandler:")]
		void RegisterItemForTypeIdentifier (string typeIdentifier, NSItemProviderLoadHandler loadHandler);

		[Export ("hasItemConformingToTypeIdentifier:")]
		bool HasItemConformingTo (string typeIdentifier);

		[Export ("loadItemForTypeIdentifier:options:completionHandler:")]
		void LoadItem (string typeIdentifier, [NullAllowed] NSDictionary options, [NullAllowed] Action<NSObject,NSError> completionHandler);

		[Field ("NSItemProviderPreferredImageSizeKey")]
		NSString PreferredImageSizeKey { get; }		

		[Export ("setPreviewImageHandler:")]
		void SetPreviewImageHandler (NSItemProviderLoadHandler handler);

		[Export ("loadPreviewImageWithOptions:completionHandler:")]
		void LoadPreviewImage (NSDictionary options, Action<NSObject,NSError> completionHandler);

		[Field ("NSItemProviderErrorDomain")]
		NSString ErrorDomain { get; }

#if MONOMAC
		[Mac (10,10)]
		[Export ("sourceFrame")]
		CGRect SourceFrame { get; }

		[Mac (10,10)]
		[Export ("containerFrame")]
		CGRect ContainerFrame { get; }

		[Mac (10,10)]
		[Export ("preferredPresentationSize")]
		CGSize PreferredPresentationSize { get; }
#endif
	}

#if XAMCORE_2_0
	[Static]
#endif
	[iOS (8,0), Mac (10,10, onlyOn64: true)]
	public partial interface NSJavaScriptExtension {
		[Field ("NSExtensionJavaScriptPreprocessingResultsKey")]
		NSString PreprocessingResultsKey { get; }

		[Field ("NSExtensionJavaScriptFinalizeArgumentKey")]
		NSString FinalizeArgumentKey { get; }
	}

	[iOS (8,0), Mac (10,10)]
	public interface NSTypeIdentifier {
		[Field ("NSTypeIdentifierDateText")]
		NSString DateText { get; }

		[Field ("NSTypeIdentifierAddressText")]
		NSString AddressText { get; }

		[Field ("NSTypeIdentifierPhoneNumberText")]
		NSString PhoneNumberText { get; }

		[Field ("NSTypeIdentifierTransitInformationText")]
		NSString TransitInformationText { get; }
	}
		
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // `init` returns a null handle
	public interface NSMethodSignature {
		[Static]
		[Export ("signatureWithObjCTypes:")]
		NSMethodSignature FromObjcTypes (IntPtr utf8objctypes);
		
		[Export ("numberOfArguments")]
		nuint NumberOfArguments { get; }

		[Export ("frameLength")]
		nuint FrameLength { get; }

		[Export ("methodReturnLength")]
		nuint MethodReturnLength { get; }

		[Export ("isOneway")]
		bool IsOneway { get; }

		[Export ("getArgumentTypeAtIndex:")]
		IntPtr GetArgumentType (nuint index);

		[Export ("methodReturnType")]
		IntPtr MethodReturnType { get; }
	}

	[Since (5,0)]
	[BaseType (typeof (NSObject), Name="NSJSONSerialization")]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** +[NSJSONSerialization allocWithZone:]: Do not create instances of NSJSONSerialization in this release
	[DisableDefaultCtor]
	interface NSJsonSerialization {
		[Static]
		[Export ("isValidJSONObject:")]
		bool IsValidJSONObject (NSObject obj);

		[Static]
		[Export ("dataWithJSONObject:options:error:")]
		NSData Serialize (NSObject obj, NSJsonWritingOptions opt, out NSError error);

		[Static]
		[Export ("JSONObjectWithData:options:error:")]
		NSObject Deserialize (NSData data, NSJsonReadingOptions opt, out NSError error);

		[Static]
		[Export ("writeJSONObject:toStream:options:error:")]
		nint Serialize (NSObject obj, NSOutputStream stream, NSJsonWritingOptions opt, out NSError error);

		[Static]
		[Export ("JSONObjectWithStream:options:error:")]
		NSObject Deserialize (NSInputStream stream, NSJsonReadingOptions opt, out NSError error);

	}
	
	[BaseType (typeof (NSIndexSet))]
	public interface NSMutableIndexSet : NSSecureCoding {
		[Export ("initWithIndex:")]
		IntPtr Constructor (nuint index);

		[Export ("initWithIndexSet:")]
		IntPtr Constructor (NSIndexSet other);

		[Export ("addIndexes:")]
		void Add (NSIndexSet other);

		[Export ("removeIndexes:")]
		void Remove (NSIndexSet other);

		[Export ("removeAllIndexes")]
		void Clear ();

		[Export ("addIndex:")]
		void Add (nuint index);

		[Export ("removeIndex:")]
		void Remove (nuint index);

		[Export ("shiftIndexesStartingAtIndex:by:")]
		void ShiftIndexes (nuint startIndex, nint delta);

		[Export ("addIndexesInRange:")]
		void AddIndexesInRange (NSRange range);

		[Export ("removeIndexesInRange:")]
		void RemoveIndexesInRange (NSRange range);
	}

	[NoWatch]
#if XAMCORE_3_0
	[DisableDefaultCtor] // the instance just crash when trying to call selectors
#endif
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSNetServiceDelegate)})]
	public interface NSNetService {
		[DesignatedInitializer]
		[Export ("initWithDomain:type:name:port:")]
		IntPtr Constructor (string domain, string type, string name, int /* int, not NSInteger */ port);

		[Export ("initWithDomain:type:name:")]
		IntPtr Constructor (string domain, string type, string name);
		
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSNetServiceDelegate Delegate { get; set; }

		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, string forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, string forMode);

		[Export ("domain", ArgumentSemantic.Copy)]
		string Domain { get; }

		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		[Export ("addresses", ArgumentSemantic.Copy)]
		NSData [] Addresses { get; }

		[Export ("port")]
		nint Port { get; }

		[Export ("publish")]
		void Publish ();

		[Export ("publishWithOptions:")]
		void Publish (NSNetServiceOptions options);

		[Export ("resolve")]
		[Availability (Introduced = Platform.iOS_2_0 | Platform.Mac_10_2, Deprecated = Platform.iOS_2_0 | Platform.Mac_10_4, Message = "Use Resolve (double) instead")]
		[NoWatch]
		void Resolve ();

		[Export ("resolveWithTimeout:")]
		void Resolve (double timeOut);

		[Export ("stop")]
		void Stop ();

		[Static, Export ("dictionaryFromTXTRecordData:")]
		NSDictionary DictionaryFromTxtRecord (NSData data);
		
		[Static, Export ("dataFromTXTRecordDictionary:")]
		NSData DataFromTxtRecord (NSDictionary dictionary);
		
		[Export ("hostName", ArgumentSemantic.Copy)]
		string HostName { get; }

		[Export ("getInputStream:outputStream:")]
		bool GetStreams (out NSInputStream inputStream, out NSOutputStream outputStream);
		
		[Export ("TXTRecordData")]
		NSData GetTxtRecordData ();

		[Export ("setTXTRecordData:")]
		bool SetTxtRecordData (NSData data);

		//NSData TxtRecordData { get; set; }

		[Export ("startMonitoring")]
		void StartMonitoring ();

		[Export ("stopMonitoring")]
		void StopMonitoring ();

		[iOS (7,0), Mac (10,10)]
		[Export ("includesPeerToPeer")]
		bool IncludesPeerToPeer { get; set; }
	}

	[NoWatch]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	public interface NSNetServiceDelegate {
		[Export ("netServiceWillPublish:")]
		void WillPublish (NSNetService sender);

		[Export ("netServiceDidPublish:")]
		void Published (NSNetService sender);

		[Export ("netService:didNotPublish:"), EventArgs ("NSNetServiceError")]
		void PublishFailure (NSNetService sender, NSDictionary errors);

		[Export ("netServiceWillResolve:")]
		void WillResolve (NSNetService sender);

		[Export ("netServiceDidResolveAddress:")]
		void AddressResolved (NSNetService sender);

		[Export ("netService:didNotResolve:"), EventArgs ("NSNetServiceError")]
		void ResolveFailure (NSNetService sender, NSDictionary errors);

		[Export ("netServiceDidStop:")]
		void Stopped (NSNetService sender);

		[Export ("netService:didUpdateTXTRecordData:"), EventArgs ("NSNetServiceData")]
		void UpdatedTxtRecordData (NSNetService sender, NSData data);

		[Since (7,0)]
		[Export ("netService:didAcceptConnectionWithInputStream:outputStream:"), EventArgs ("NSNetServiceConnection")]
		void DidAcceptConnection (NSNetService sender, NSInputStream inputStream, NSOutputStream outputStream);
	}

	[NoWatch]
	[BaseType (typeof (NSObject),
		   Delegates=new string [] {"WeakDelegate"},
		   Events=new Type [] {typeof (NSNetServiceBrowserDelegate)})]
	public interface NSNetServiceBrowser {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSNetServiceBrowserDelegate Delegate { get; set; }

		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, string forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, string forMode);

		[Export ("searchForBrowsableDomains")]
		void SearchForBrowsableDomains ();

		[Export ("searchForRegistrationDomains")]
		void SearchForRegistrationDomains ();

		[Export ("searchForServicesOfType:inDomain:")]
		void SearchForServices (string type, string domain);

		[Export ("stop")]
		void Stop ();

		[iOS (7,0), Mac(10,10)]
		[Export ("includesPeerToPeer")]
		bool IncludesPeerToPeer { get; set; }
	}

	[NoWatch]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	public interface NSNetServiceBrowserDelegate {
		[Export ("netServiceBrowserWillSearch:")]
		void SearchStarted (NSNetServiceBrowser sender);
		
		[Export ("netServiceBrowserDidStopSearch:")]
		void SearchStopped (NSNetServiceBrowser sender);
		
		[Export ("netServiceBrowser:didNotSearch:"), EventArgs ("NSNetServiceError")]
		void NotSearched (NSNetServiceBrowser sender, NSDictionary errors);
		
		[Export ("netServiceBrowser:didFindDomain:moreComing:"), EventArgs ("NSNetDomain")]
		void FoundDomain (NSNetServiceBrowser sender, string domain, bool moreComing);
		
		[Export ("netServiceBrowser:didFindService:moreComing:"), EventArgs ("NSNetService")]
		void FoundService (NSNetServiceBrowser sender, NSNetService service, bool moreComing);
		
		[Export ("netServiceBrowser:didRemoveDomain:moreComing:"), EventArgs ("NSNetDomain")]
		void DomainRemoved (NSNetServiceBrowser sender, string domain, bool moreComing);
		
		[Export ("netServiceBrowser:didRemoveService:moreComing:"), EventArgs ("NSNetService")]
		void ServiceRemoved (NSNetServiceBrowser sender, NSNetService service, bool moreComing);
	}

	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: *** -[NSConcreteNotification init]: should never be used
	[DisableDefaultCtor] // added in iOS7 but header files says "do not invoke; not a valid initializer for this class"
	public interface NSNotification : NSCoding, NSCopying {
		[Export ("name")]
		// Null not allowed
		string Name { get; }

		[Export ("object")]
		[NullAllowed]
		NSObject Object { get; }

		[Export ("userInfo")]
		[NullAllowed]
		NSDictionary UserInfo { get; }

		[Export ("notificationWithName:object:")][Static]
		NSNotification FromName (string name, [NullAllowed] NSObject obj);

		[Export ("notificationWithName:object:userInfo:")][Static]
		NSNotification FromName (string name,[NullAllowed]  NSObject obj, [NullAllowed] NSDictionary userInfo);
		
	}

	[BaseType (typeof (NSObject))]
	public interface NSNotificationCenter {
		[Static][Export ("defaultCenter")]
		NSNotificationCenter DefaultCenter { get; }
	
		[Export ("addObserver:selector:name:object:")]
		[PostSnippet ("AddObserverToList (observer, aName, anObject);")]
		void AddObserver (NSObject observer, Selector aSelector, [NullAllowed] NSString aName, [NullAllowed] NSObject anObject);
	
		[Export ("postNotification:")]
		void PostNotification (NSNotification notification);
	
		[Export ("postNotificationName:object:")]
		void PostNotificationName (string aName, [NullAllowed] NSObject anObject);
	
		[Export ("postNotificationName:object:userInfo:")]
		void PostNotificationName (string aName, [NullAllowed] NSObject anObject, [NullAllowed] NSDictionary aUserInfo);
	
		[Export ("removeObserver:")]
		[PostSnippet ("RemoveObserversFromList (observer, null, null);")]
		void RemoveObserver (NSObject observer);
	
		[Export ("removeObserver:name:object:")]
		[PostSnippet ("RemoveObserversFromList (observer, aName, anObject);")]
		void RemoveObserver (NSObject observer, [NullAllowed] string aName, [NullAllowed] NSObject anObject);

		[Since (4,0)]
		[Export ("addObserverForName:object:queue:usingBlock:")]
#if XAMCORE_2_0
		NSObject AddObserver ([NullAllowed] string name, [NullAllowed] NSObject obj, [NullAllowed] NSOperationQueue queue, Action<NSNotification> handler);
#else
		NSObject AddObserver ([NullAllowed] string name, [NullAllowed] NSObject obj, [NullAllowed] NSOperationQueue queue, NSNotificationHandler handler);
#endif
	}

#if MONOMAC
	[Mac (10, 10)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	public interface NSDistributedLock
	{
		[Static]
		[Export ("lockWithPath:")]
		[return: NullAllowed]
		NSDistributedLock FromPath (string path);

		[Export ("initWithPath:")]
		[DesignatedInitializer]
		IntPtr Constructor (string path);

		[Export ("tryLock")]
		bool TryLock ();

		[Export ("unlock")]
		void Unlock ();

		[Export ("breakLock")]
		void BreakLock ();

		[Export ("lockDate", ArgumentSemantic.Copy)]
		NSDate LockDate { get; }
	}

	[BaseType (typeof (NSNotificationCenter))]
	public interface NSDistributedNotificationCenter {
		[Static]
		[Export ("defaultCenter")]
		NSObject DefaultCenter { get; }

		[Export ("addObserver:selector:name:object:suspensionBehavior:")]
		void AddObserver (NSObject observer, Selector selector, [NullAllowed] string notificationName, [NullAllowed] string notificationSenderc, NSNotificationSuspensionBehavior suspensionBehavior);

		[Export ("postNotificationName:object:userInfo:deliverImmediately:")]
		void PostNotificationName (string name, [NullAllowed] string anObject, [NullAllowed] NSDictionary userInfo, bool deliverImmediately);
		
		[Export ("postNotificationName:object:userInfo:options:")]
		void PostNotificationName (string name, [NullAllowed] string anObjecb, [NullAllowed] NSDictionary userInfo, NSNotificationFlags options);

		[Export ("addObserver:selector:name:object:")]
		void AddObserver (NSObject observer, Selector aSelector, [NullAllowed] string aName, [NullAllowed] NSObject anObject);

		[Export ("postNotificationName:object:")]
		void PostNotificationName (string aName, [NullAllowed] string anObject);

		[Export ("postNotificationName:object:userInfo:")]
		void PostNotificationName (string aName, [NullAllowed] string anObject, [NullAllowed] NSDictionary aUserInfo);

		[Export ("removeObserver:name:object:")]
		void RemoveObserver (NSObject observer, [NullAllowed] string aName, [NullAllowed] NSObject anObject);

		//Detected properties
		[Export ("suspended")]
		bool Suspended { get; set; }
		
		[Field ("NSLocalNotificationCenterType")]
		NSString NSLocalNotificationCenterType {get;}
	}
#endif
	
	[BaseType (typeof (NSObject))]
	public interface NSNotificationQueue {
		[Static][IsThreadStatic]
		[Export ("defaultQueue")]
		NSNotificationQueue DefaultQueue { get; }

		[DesignatedInitializer]
		[Export ("initWithNotificationCenter:")]
		IntPtr Constructor (NSNotificationCenter notificationCenter);

		[Export ("enqueueNotification:postingStyle:")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle);

		[Export ("enqueueNotification:postingStyle:coalesceMask:forModes:")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, string [] modes);

		[Export ("dequeueNotificationsMatching:coalesceMask:")]
		void DequeueNotificationsMatchingcoalesceMask (NSNotification notification, NSNotificationCoalescing coalesceMask);
	}

#if !XAMCORE_2_0
	public delegate void NSNotificationHandler (NSNotification notification);
#endif

	[BaseType (typeof (NSObject))]
	// init returns NIL
	[DisableDefaultCtor]
	public partial interface NSValue : NSSecureCoding, NSCopying {
		[Export ("getValue:")]
		void StoreValueAtAddress (IntPtr value);

		[Export ("objCType")][Internal]
		IntPtr ObjCTypePtr ();
		
		//[Export ("initWithBytes:objCType:")][Internal]
		//NSValue InitFromBytes (IntPtr byte_ptr, IntPtr char_ptr_type);
		//[Export ("valueWithBytes:objCType:")][Static][Internal]
		//+ (NSValue *)valueWithBytes:(const void *)value objCType:(const char *)type;
		//+ (NSValue *)value:(const void *)value withObjCType:(const char *)type;

		[Static]
		[Export ("valueWithNonretainedObject:")]
		NSValue ValueFromNonretainedObject (NSObject anObject);
	
		[Export ("nonretainedObjectValue")]
		NSObject NonretainedObjectValue { get; }
	
		[Static]
		[Export ("valueWithPointer:")]
		NSValue ValueFromPointer (IntPtr pointer);
	
		[Export ("pointerValue")]
		IntPtr PointerValue { get; }
	
		[Export ("isEqualToValue:")]
		bool IsEqualTo (NSValue value);
		
		[Export ("valueWithRange:")][Static]
		NSValue FromRange(NSRange range);

		[Export ("rangeValue")]
		NSRange RangeValue { get; }

#if MONOMAC
		[Static, Export ("valueWithCMTime:"), Lion]
		NSValue FromCMTime (CMTime time);
		
		[Export ("CMTimeValue"), Lion]
		CMTime CMTimeValue { get; }
		
		[Static, Export ("valueWithCMTimeMapping:"), Lion]
		NSValue FromCMTimeMapping (CMTimeMapping timeMapping);
		
		[Export ("CMTimeMappingValue"), Lion]
		CMTimeMapping CMTimeMappingValue { get; }
		
		[Static, Export ("valueWithCMTimeRange:"), Lion]
		NSValue FromCMTimeRange (CMTimeRange timeRange);
		
		[Export ("CMTimeRangeValue"), Lion]
		CMTimeRange CMTimeRangeValue { get; }

		[Export ("valueWithRect:"), Static]
		NSValue FromCGRect (CGRect rect);

		[Export ("valueWithSize:")][Static]
		NSValue FromCGSize (CGSize size);

		[Export ("valueWithPoint:")][Static]
		NSValue FromCGPoint (CGPoint point);

		[Export ("rectValue")]
		CGRect CGRectValue { get; }

		[Export ("sizeValue")]
		CGSize CGSizeValue { get; }

		[Export ("pointValue")]
		CGPoint CGPointValue { get; }

#if XAMCORE_2_0
		[Mac (10,9, onlyOn64 : true)] // The header doesn't say, but the rest of the API from the same file (MKGeometry.h) was introduced in 10.9
		[Static, Export ("valueWithMKCoordinate:")]
		NSValue FromMKCoordinate (XamCore.CoreLocation.CLLocationCoordinate2D coordinate);

		[Mac (10,9, onlyOn64 : true)] // The header doesn't say, but the rest of the API from the same file (MKGeometry.h) was introduced in 10.9
		[Static, Export ("valueWithMKCoordinateSpan:")]
		NSValue FromMKCoordinateSpan (XamCore.MapKit.MKCoordinateSpan coordinateSpan);
#endif
#else
#if !WATCH
		[Static, Export ("valueWithCMTime:"), Since (4,0)]
		NSValue FromCMTime (CMTime time);
		
		[Export ("CMTimeValue"), Since (4,0)]
		CMTime CMTimeValue { get; }
		
		[Static, Export ("valueWithCMTimeMapping:"), Since (4,0)]
		NSValue FromCMTimeMapping (CMTimeMapping timeMapping);
		
		[Export ("CMTimeMappingValue"), Since (4,0)]
		CMTimeMapping CMTimeMappingValue { get; }
		
		[Static, Export ("valueWithCMTimeRange:"), Since (4,0)]
		NSValue FromCMTimeRange (CMTimeRange timeRange);
		
		[Export ("CMTimeRangeValue"), Since (4,0)]
		CMTimeRange CMTimeRangeValue { get; }
#endif
		
		[Export ("CGAffineTransformValue")]
		XamCore.CoreGraphics.CGAffineTransform CGAffineTransformValue { get; }
		
		[Export ("UIEdgeInsetsValue")]
		UIKit.UIEdgeInsets UIEdgeInsetsValue { get; }

		[Export ("valueWithCGAffineTransform:")][Static]
		NSValue FromCGAffineTransform (XamCore.CoreGraphics.CGAffineTransform tran);

		[Export ("valueWithUIEdgeInsets:")][Static]
		NSValue FromUIEdgeInsets (UIKit.UIEdgeInsets insets);

		[Since (5,0)]
		[Export ("valueWithUIOffset:")][Static]
		NSValue FromUIOffset (UIKit.UIOffset insets);

		[Since (5,0)]
		[Export ("UIOffsetValue")]
		UIOffset UIOffsetValue { get; }

		[Export ("valueWithCGRect:")][Static]
		NSValue FromCGRect (CGRect rect);

		[Export ("CGRectValue")]
		CGRect CGRectValue { get; }

		[Export ("valueWithCGSize:")][Static]
		NSValue FromCGSize (CGSize size);

		[Export ("CGSizeValue")]
		CGSize CGSizeValue { get; }

		[Export ("CGPointValue")]
		CGPoint CGPointValue { get; }
		
		[Export ("valueWithCGPoint:")][Static]
		NSValue FromCGPoint (CGPoint point);

		// from UIGeometry.h - those are in iOS8 only (even if the header is silent about them)
		// and not in OSX 10.10

		[iOS (8,0)]
		[Export ("CGVectorValue")]
		CGVector CGVectorValue { get; }

		[iOS (8,0)]
		[Static, Export ("valueWithCGVector:")]
		NSValue FromCGVector (CGVector vector);

		// Maybe we should include this inside mapkit.cs instead (it's a partial interface, so that's trivial)?

		[TV (9,2)]
		[Since (6,0)]
		[Static, Export ("valueWithMKCoordinate:")]
		NSValue FromMKCoordinate (XamCore.CoreLocation.CLLocationCoordinate2D coordinate);

		[TV (9,2)]
		[Since (6,0)]
		[Static, Export ("valueWithMKCoordinateSpan:")]
		NSValue FromMKCoordinateSpan (XamCore.MapKit.MKCoordinateSpan coordinateSpan);

		[TV (9,2)]
		[Since (6,0)]
		[Export ("MKCoordinateValue")]
		XamCore.CoreLocation.CLLocationCoordinate2D CoordinateValue { get; }

		[TV (9,2)]
		[Since (6,0)]
		[Export ("MKCoordinateSpanValue")]
		XamCore.MapKit.MKCoordinateSpan CoordinateSpanValue { get; }
#endif

#if !WATCH
		[Export ("valueWithCATransform3D:")][Static]
		NSValue FromCATransform3D (XamCore.CoreAnimation.CATransform3D transform);

		[Export ("CATransform3DValue")]
		XamCore.CoreAnimation.CATransform3D CATransform3DValue { get; }
#endif

#if !WATCH
		#region SceneKit Additions

		[Mac (10,8), iOS (8,0)]
		[Static, Export ("valueWithSCNVector3:")]
		NSValue FromVector (SCNVector3 vector);

		[Mac (10,8), iOS (8,0)]
		[Export ("SCNVector3Value")]
		SCNVector3 Vector3Value { get; }

		[Mac (10,8), iOS (8,0)]
		[Static, Export ("valueWithSCNVector4:")]
		NSValue FromVector (SCNVector4 vector);

		[Mac (10,8), iOS (8,0)]
		[Export ("SCNVector4Value")]
		SCNVector4 Vector4Value { get; }

		[Mac (10,10), iOS (8,0)]
		[Static, Export ("valueWithSCNMatrix4:")]
		NSValue FromSCNMatrix4 (SCNMatrix4 matrix);

		[Mac (10,10), iOS (8,0)]
		[Export ("SCNMatrix4Value")]
		SCNMatrix4 SCNMatrix4Value { get; }

		#endregion
#endif
	}

	[BaseType (typeof (NSObject))]
#if !MONOMAC || !XAMCORE_4_0
	// there were some, partial bindings in foundation-desktop.cs which did not define it as abstract for XM :(
	[Abstract] // Apple docs: NSValueTransformer is an abstract class...
#endif
	interface NSValueTransformer {
		[Static]
		[Export ("setValueTransformer:forName:")]
		void SetValueTransformer ([NullAllowed] NSValueTransformer transformer, string name);

		[Static]
		[Export ("valueTransformerForName:")]
		[return: NullAllowed]
		NSValueTransformer GetValueTransformer (string name);

		[Static]
		[Export ("valueTransformerNames")]
		string[] ValueTransformerNames { get; }

		[Static]
		[Export ("transformedValueClass")]
		Class TransformedValueClass { get; }

		[Static]
		[Export ("allowsReverseTransformation")]
		bool AllowsReverseTransformation { get; }

		[Export ("transformedValue:")]
		[return: NullAllowed]
		NSObject TransformedValue ([NullAllowed] NSObject value);

		[Export ("reverseTransformedValue:")]
		[return: NullAllowed]
		NSObject ReverseTransformedValue ([NullAllowed] NSObject value);

#if IOS && !XAMCORE_4_0
		[iOS (9, 3)]
		[Notification]
		[Obsolete ("Use NSUserDefaults.SizeLimitExceededNotification instead")]
		[Field ("NSUserDefaultsSizeLimitExceededNotification")]
		NSString SizeLimitExceededNotification { get; }

		[iOS (9, 3)]
		[Notification]
		[Obsolete ("Use NSUserDefaults.DidChangeAccountsNotification instead")]
		[Field ("NSUbiquitousUserDefaultsDidChangeAccountsNotification")]
		NSString DidChangeAccountsNotification { get; }

		[iOS (9, 3)]
		[Notification]
		[Obsolete ("Use NSUserDefaults.CompletedInitialSyncNotification instead")]
		[Field ("NSUbiquitousUserDefaultsCompletedInitialSyncNotification")]
		NSString CompletedInitialSyncNotification { get; }

		[Notification]
		[Obsolete ("Use NSUserDefaults.DidChangeNotification instead")]
		[Field ("NSUserDefaultsDidChangeNotification")]
		NSString UserDefaultsDidChangeNotification { get; }
#endif

		[Field ("NSNegateBooleanTransformerName")]
		NSString BooleanTransformerName { get; }

		[Field ("NSIsNilTransformerName")]
		NSString IsNilTransformerName { get; }

		[Field ("NSIsNotNilTransformerName")]
		NSString IsNotNilTransformerName { get; }

		[Field ("NSUnarchiveFromDataTransformerName")]
		NSString UnarchiveFromDataTransformerName { get; }

		[Field ("NSKeyedUnarchiveFromDataTransformerName")]
		NSString KeyedUnarchiveFromDataTransformerName { get; }
	}
	
	[BaseType (typeof (NSValue))]
	// init returns NIL
	[DisableDefaultCtor]
	public interface NSNumber {
		[Export ("charValue")]
		sbyte SByteValue { get; }
	
		[Export ("unsignedCharValue")]
		byte ByteValue { get; }
	
		[Export ("shortValue")]
		short Int16Value { get; }
	
		[Export ("unsignedShortValue")]
		ushort UInt16Value { get; }
	
		[Export ("intValue")]
		int Int32Value { get; }
	
		[Export ("unsignedIntValue")]
		uint UInt32Value { get; } 
	
		[Export ("longValue")]
		nint LongValue { get; }
		
		[Export ("unsignedLongValue")]
		nuint UnsignedLongValue { get; }
	
		[Export ("longLongValue")]
		long Int64Value { get; }
	
		[Export ("unsignedLongLongValue")]
		ulong UInt64Value { get; }
	
		[Export ("floatValue")]
		float FloatValue { get; } /* float, not CGFloat */
	
		[Export ("doubleValue")]
		double DoubleValue { get; }

		[Export ("decimalValue")]
		NSDecimal NSDecimalValue { get; }
	
		[Export ("boolValue")]
		bool BoolValue { get; }

		[Export ("integerValue")]
#if XAMCORE_2_0
		nint NIntValue { get; }
#else
		nint IntValue { get; }
#endif

		[Export ("unsignedIntegerValue")]
#if XAMCORE_2_0
		nuint NUIntValue { get; }
#else
		nuint UnsignedIntegerValue { get; }
#endif

		[Export ("stringValue")]
		string StringValue { get; }

		[Export ("compare:")]
		nint Compare (NSNumber otherNumber);
	
#if XAMCORE_2_0
		[Internal] // Equals(object) or IEquatable<T>'s Equals(NSNumber)
#endif
		[Export ("isEqualToNumber:")]
		bool IsEqualToNumber (NSNumber number);
	
		[Export ("descriptionWithLocale:")]
		string DescriptionWithLocale (NSLocale locale);

		[DesignatedInitializer]
		[Export ("initWithChar:")]
		IntPtr Constructor (sbyte value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedChar:")]
		IntPtr Constructor (byte value);
	
		[DesignatedInitializer]
		[Export ("initWithShort:")]
		IntPtr Constructor (short value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedShort:")]
		IntPtr Constructor (ushort value);
	
		[DesignatedInitializer]
		[Export ("initWithInt:")]
		IntPtr Constructor (int /* int, not NSInteger */ value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedInt:")]
		IntPtr Constructor (uint /* unsigned int, not NSUInteger */value);
	
		//[Export ("initWithLong:")]
		//IntPtr Constructor (long value);
		//
		//[Export ("initWithUnsignedLong:")]
		//IntPtr Constructor (ulong value);
	
		[DesignatedInitializer]
		[Export ("initWithLongLong:")]
		IntPtr Constructor (long value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedLongLong:")]
		IntPtr Constructor (ulong value);
	
		[DesignatedInitializer]
		[Export ("initWithFloat:")]
		IntPtr Constructor (float /* float, not CGFloat */ value);
	
		[DesignatedInitializer]
		[Export ("initWithDouble:")]
		IntPtr Constructor (double value);
	
		[DesignatedInitializer]
		[Export ("initWithBool:")]
		IntPtr Constructor (bool value);

#if XAMCORE_2_0
		[DesignatedInitializer]
		[Export ("initWithInteger:")]
		IntPtr Constructor (nint value);

		[DesignatedInitializer]
		[Export ("initWithUnsignedInteger:")]
		IntPtr Constructor (nuint value);
#endif
	
		[Export ("numberWithChar:")][Static]
		NSNumber FromSByte (sbyte value);
	
		[Static]
		[Export ("numberWithUnsignedChar:")]
		NSNumber FromByte (byte value);
	
		[Static]
		[Export ("numberWithShort:")]
		NSNumber FromInt16 (short value);
	
		[Static]
		[Export ("numberWithUnsignedShort:")]
		NSNumber FromUInt16 (ushort value);
	
		[Static]
		[Export ("numberWithInt:")]
		NSNumber FromInt32 (int /* int, not NSInteger */ value);
	
		[Static]
		[Export ("numberWithUnsignedInt:")]
		NSNumber FromUInt32 (uint /* unsigned int, not NSUInteger */ value);

		[Static]
		[Export ("numberWithLong:")]
		NSNumber FromLong (nint value);
		//
		[Static]
		[Export ("numberWithUnsignedLong:")]
		NSNumber FromUnsignedLong (nuint value);
	
		[Static]
		[Export ("numberWithLongLong:")]
		NSNumber FromInt64 (long value);
	
		[Static]
		[Export ("numberWithUnsignedLongLong:")]
		NSNumber FromUInt64 (ulong value);
	
		[Static]
		[Export ("numberWithFloat:")]
		NSNumber FromFloat (float /* float, not CGFloat */ value);
	
		[Static]
		[Export ("numberWithDouble:")]
		NSNumber FromDouble (double value);
	
		[Static]
		[Export ("numberWithBool:")]
		NSNumber FromBoolean (bool value);

#if !XAMCORE_2_0
		[Internal]
#endif
		[Static]
		[Export ("numberWithInteger:")]
		NSNumber FromNInt (nint value);

#if !XAMCORE_2_0
		[Internal]
#endif
		[Static]
		[Export ("numberWithUnsignedInteger:")]
		NSNumber FromNUInt (nuint value);
	}


	[BaseType (typeof (NSFormatter))]
	interface NSNumberFormatter {
		[Export ("stringFromNumber:")]
		string StringFromNumber (NSNumber number);

		[Export ("numberFromString:")]
		NSNumber NumberFromString (string text);

		[Static]
		[Export ("localizedStringFromNumber:numberStyle:")]
		string LocalizedStringFromNumbernumberStyle (NSNumber num, NSNumberFormatterStyle nstyle);

		//Detected properties
		[Export ("numberStyle")]
		NSNumberFormatterStyle NumberStyle { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("generatesDecimalNumbers")]
		bool GeneratesDecimalNumbers { get; set; }

		[Export ("formatterBehavior")]
		NSNumberFormatterBehavior FormatterBehavior { get; set; }

		[Static]
		[Export ("defaultFormatterBehavior")]
		NSNumberFormatterBehavior DefaultFormatterBehavior { get; set; }

		[Export ("negativeFormat")]
		string NegativeFormat { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNegativeValues", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNegativeValues { get; set; }

		[Export ("positiveFormat")]
		string PositiveFormat { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForPositiveValues", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForPositiveValues { get; set; }

		[Export ("allowsFloats")]
		bool AllowsFloats { get; set; }

		[Export ("decimalSeparator")]
		string DecimalSeparator { get; set; }

		[Export ("alwaysShowsDecimalSeparator")]
		bool AlwaysShowsDecimalSeparator { get; set; }

		[Export ("currencyDecimalSeparator")]
		string CurrencyDecimalSeparator { get; set; }

		[Export ("usesGroupingSeparator")]
		bool UsesGroupingSeparator { get; set; }

		[Export ("groupingSeparator")]
		string GroupingSeparator { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("zeroSymbol")]
		string ZeroSymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForZero", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForZero { get; set; }

		[Export ("nilSymbol")]
		string NilSymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNil", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNil { get; set; }

		[Export ("notANumberSymbol")]
		string NotANumberSymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNotANumber", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNotANumber { get; set; }

		[Export ("positiveInfinitySymbol")]
		string PositiveInfinitySymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForPositiveInfinity", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForPositiveInfinity { get; set; }

		[Export ("negativeInfinitySymbol")]
		string NegativeInfinitySymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNegativeInfinity", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNegativeInfinity { get; set; }

		[Export ("positivePrefix")]
		string PositivePrefix { get; set; }

		[Export ("positiveSuffix")]
		string PositiveSuffix { get; set; }

		[Export ("negativePrefix")]
		string NegativePrefix { get; set; }

		[Export ("negativeSuffix")]
		string NegativeSuffix { get; set; }

		[Export ("currencyCode")]
		string CurrencyCode { get; set; }

		[Export ("currencySymbol")]
		string CurrencySymbol { get; set; }

		[Export ("internationalCurrencySymbol")]
		string InternationalCurrencySymbol { get; set; }

		[Export ("percentSymbol")]
		string PercentSymbol { get; set; }

		[Export ("perMillSymbol")]
		string PerMillSymbol { get; set; }

		[Export ("minusSign")]
		string MinusSign { get; set; }

		[Export ("plusSign")]
		string PlusSign { get; set; }

		[Export ("exponentSymbol")]
		string ExponentSymbol { get; set; }

		[Export ("groupingSize")]
		nuint GroupingSize { get; set; }

		[Export ("secondaryGroupingSize")]
		nuint SecondaryGroupingSize { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("multiplier", ArgumentSemantic.Copy)]
		NSNumber Multiplier { get; set; }

		[Export ("formatWidth")]
		nuint FormatWidth { get; set; }

		[Export ("paddingCharacter")]
		string PaddingCharacter { get; set; }

		[Export ("paddingPosition")]
		NSNumberFormatterPadPosition PaddingPosition { get; set; }

		[Export ("roundingMode")]
		NSNumberFormatterRoundingMode RoundingMode { get; set; }

		[Export ("roundingIncrement", ArgumentSemantic.Copy)]
		NSNumber RoundingIncrement { get; set; }

		[Export ("minimumIntegerDigits")]
		nint MinimumIntegerDigits { get; set; }

		[Export ("maximumIntegerDigits")]
		nint MaximumIntegerDigits { get; set; }

		[Export ("minimumFractionDigits")]
		nint MinimumFractionDigits { get; set; }

		[Export ("maximumFractionDigits")]
		nint MaximumFractionDigits { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("minimum", ArgumentSemantic.Copy)]
		NSNumber Minimum { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("maximum", ArgumentSemantic.Copy)]
		NSNumber Maximum { get; set; }

		[Export ("currencyGroupingSeparator")]
		string CurrencyGroupingSeparator { get; set; }

		[Export ("lenient")]
		bool Lenient { [Bind ("isLenient")]get; set; }

		[Export ("usesSignificantDigits")]
		bool UsesSignificantDigits { get; set; }

		[Export ("minimumSignificantDigits")]
		nuint MinimumSignificantDigits { get; set; }

		[Export ("maximumSignificantDigits")]
		nuint MaximumSignificantDigits { get; set; }

		[Export ("partialStringValidationEnabled")]
		bool PartialStringValidationEnabled { [Bind ("isPartialStringValidationEnabled")]get; set; }
	}

	[BaseType (typeof (NSNumber))]
	public interface NSDecimalNumber : NSSecureCoding {
		[Export ("initWithMantissa:exponent:isNegative:")]
		IntPtr Constructor (long mantissa, short exponent, bool isNegative);
		
		[DesignatedInitializer]
		[Export ("initWithDecimal:")]
		IntPtr Constructor (NSDecimal dec);

		[Export ("initWithString:")]
		IntPtr Constructor (string numberValue);

		[Export ("initWithString:locale:")]
		IntPtr Constructor (string numberValue, NSObject locale);

		[Export ("descriptionWithLocale:")]
		[Override]
		string DescriptionWithLocale (NSLocale locale);

		[Export ("decimalValue")]
		NSDecimal NSDecimalValue { get; }

		[Export ("zero")][Static]
		NSDecimalNumber Zero { get; }

		[Export ("one")][Static]
		NSDecimalNumber One { get; }
		
		[Export ("minimumDecimalNumber")][Static]
		NSDecimalNumber MinValue { get; }
		
		[Export ("maximumDecimalNumber")][Static]
		NSDecimalNumber MaxValue { get; }

		[Export ("notANumber")][Static]
		NSDecimalNumber NaN { get; }

		//
		// All the behavior ones require:
		// id <NSDecimalNumberBehaviors>)behavior;

		[Export ("decimalNumberByAdding:")]
		NSDecimalNumber Add (NSDecimalNumber d);

		[Export ("decimalNumberByAdding:withBehavior:")]
		NSDecimalNumber Add (NSDecimalNumber d, NSObject Behavior);

		[Export ("decimalNumberBySubtracting:")]
		NSDecimalNumber Subtract (NSDecimalNumber d);

		[Export ("decimalNumberBySubtracting:withBehavior:")]
		NSDecimalNumber Subtract (NSDecimalNumber d, NSObject Behavior);
		
		[Export ("decimalNumberByMultiplyingBy:")]
		NSDecimalNumber Multiply (NSDecimalNumber d);

		[Export ("decimalNumberByMultiplyingBy:withBehavior:")]
		NSDecimalNumber Multiply (NSDecimalNumber d, NSObject Behavior);
		
		[Export ("decimalNumberByDividingBy:")]
		NSDecimalNumber Divide (NSDecimalNumber d);

		[Export ("decimalNumberByDividingBy:withBehavior:")]
		NSDecimalNumber Divide (NSDecimalNumber d, NSObject Behavior);

		[Export ("decimalNumberByRaisingToPower:")]
		NSDecimalNumber RaiseTo (nuint power);

		[Export ("decimalNumberByRaisingToPower:withBehavior:")]
		NSDecimalNumber RaiseTo (nuint power, NSObject Behavior);
		
		[Export ("decimalNumberByMultiplyingByPowerOf10:")]
		NSDecimalNumber MultiplyPowerOf10 (short power);

		[Export ("decimalNumberByMultiplyingByPowerOf10:withBehavior:")]
		NSDecimalNumber MultiplyPowerOf10 (short power, NSObject Behavior);

		[Export ("decimalNumberByRoundingAccordingToBehavior:")]
		NSDecimalNumber Rounding (NSObject behavior);

		[Export ("compare:")]
		[Override]
		nint Compare (NSNumber other);

		[Export ("defaultBehavior")][Static]
		NSObject DefaultBehavior { get; set; }

		[Export ("doubleValue")]
		[Override]
		double DoubleValue { get; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSThread {
		[Static, Export ("currentThread")]
		NSThread Current { get; }

		[Static, Export ("callStackSymbols")]
		string [] NativeCallStack { get; }

		//+ (void)detachNewThreadSelector:(SEL)selector toTarget:(id)target withObject:(id)argument;

		[Static, Export ("isMultiThreaded")]
		bool IsMultiThreaded { get; }

		//- (NSMutableDictionary *)threadDictionary;

		[Static, Export ("sleepUntilDate:")]
		void SleepUntil (NSDate date);
		
		[Static, Export ("sleepForTimeInterval:")]
		void SleepFor (double timeInterval);

		[Static, Export ("exit")]
		void Exit ();

		[Static, Export ("threadPriority"), Internal]
		double _GetPriority ();

		[Static, Export ("setThreadPriority:"), Internal]
		bool _SetPriority (double priority);

		//+ (NSArray *)callStackReturnAddresses;

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("stackSize")]
		nuint StackSize { get; set; }

		[Export ("isMainThread")]
		bool IsMainThread { get; }

		// MainThread is already used for the instance selector and we can't reuse the same name
		[Static]
		[Export ("isMainThread")]
		bool IsMain { get; }

		[Static]
		[Export ("mainThread")]
		NSThread MainThread { get; }

		[Export ("isExecuting")]
		bool IsExecuting { get; }

		[Export ("isFinished")]
		bool IsFinished { get; }

		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("start")]
		void Start ();

		[Export ("main")]
		void Main ();

		[Mac (10,10), iOS (8,0)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }
			
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface NSPort : NSCoding, NSCopying {
		[Static, Export ("port")]
		NSPort Create ();

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		NSPortDelegate Delegate { get; set; }

		[Export ("scheduleInRunLoop:forMode:")]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSString runLoopMode);

		[Export ("removeFromRunLoop:forMode:")]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSString runLoopMode);

		// Disable warning for NSMutableArray
#pragma warning disable 618
		[Export ("sendBeforeDate:components:from:reserved:")]
		bool SendBeforeDate (NSDate limitDate, [NullAllowed] NSMutableArray components, [NullAllowed] NSPort receivePort, nuint headerSpaceReserved);

		[Export ("sendBeforeDate:msgid:components:from:reserved:")]
		bool SendBeforeDate (NSDate limitDate, nuint msgID, [NullAllowed] NSMutableArray components, [NullAllowed] NSPort receivePort, nuint headerSpaceReserved);
#pragma warning restore 618
	}

	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	public interface NSPortDelegate {
		[Export ("handlePortMessage:")]
		void MessageReceived (NSPortMessage message);
	}

	[BaseType (typeof (NSObject))]
	public interface NSPortMessage {
#if MONOMAC
		[Export ("initWithSendPort:receivePort:components:")]
		IntPtr Constructor (NSPort sendPort, NSPort recvPort, NSArray components);

		[Export ("components")]
		NSArray Components { get; }

		// Apple started refusing applications that use those selectors (desk #63237)
		// The situation is a bit confusing since NSPortMessage.h is not part of iOS SDK - 
		// but the type is used (from NSPort[Delegate]) but not _itself_ documented
		// The selectors Apple *currently* dislike are removed from the iOS build

		[Export ("sendBeforeDate:")]
		bool SendBefore (NSDate date);

		[Export ("receivePort")]
		NSPort ReceivePort { get; }

		[Export ("sendPort")]
		NSPort SendPort { get; }

		[Export ("msgid")]
		uint MsgId { get; set; } /* uint32_t */
#endif
	}

	[BaseType (typeof (NSPort))]
	public interface NSMachPort {
		[DesignatedInitializer]
		[Export ("initWithMachPort:")]
		IntPtr Constructor (uint /* uint32_t */ machPort);

		[DesignatedInitializer]
		[Export ("initWithMachPort:options:")]
		IntPtr Constructor (uint /* uint32_t */ machPort, NSMachPortRights options);
		
		[Static, Export ("portWithMachPort:")]
		NSPort FromMachPort (uint /* uint32_t */ port);

		[Static, Export ("portWithMachPort:options:")]
		NSPort FromMachPort (uint /* uint32_t */ port, NSMachPortRights options);

		[Export ("machPort")]
		uint MachPort { get; } /* uint32_t */

		[Export ("removeFromRunLoop:forMode:")]
		[Override]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSString mode);

		[Export ("scheduleInRunLoop:forMode:")]
		[Override]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSString mode);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		[Override]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		NSMachPortDelegate Delegate { get; set; }
	}

	[Model, BaseType (typeof (NSPortDelegate))]
	[Protocol]
	public interface NSMachPortDelegate {
		[Export ("handleMachMessage:")]
		void MachMessageReceived (IntPtr msgHeader);
	}

	[BaseType (typeof (NSObject))]
	public interface NSProcessInfo {
		[Export ("processInfo")][Static]
		NSProcessInfo ProcessInfo { get; }

		[Export ("arguments")]
		string [] Arguments { get; }

		[Export ("environment")]
		NSDictionary Environment { get; }

		[Export ("processIdentifier")]
		int ProcessIdentifier { get; } /* int, not NSInteger */

		[Export ("globallyUniqueString")]
		string GloballyUniqueString { get; }

		[Export ("processName")]
		string ProcessName { get; set; }

		[Export ("hostName")]
		string HostName { get; }

		[Availability (Deprecated = Platform.Mac_10_10 | Platform.iOS_8_0, Message="Use OperatingSystemVersion or IsOperatingSystemAtLeastVersion")]
		[Export ("operatingSystem")]
		nint OperatingSystem { get; }

		[Availability (Deprecated = Platform.Mac_10_10 | Platform.iOS_8_0, Message="Use OperatingSystemVersionString")]
		[Export ("operatingSystemName")]
		string OperatingSystemName { get; }

		[Export ("operatingSystemVersionString")]
		string OperatingSystemVersionString { get; }

		[Export ("physicalMemory")]
		ulong PhysicalMemory { get; }
		
		[Export ("processorCount")]
		nint ProcessorCount { get; }
		
		[Export ("activeProcessorCount")]
		nint ActiveProcessorCount { get; }

		[Export ("systemUptime")]
		double SystemUptime { get; }

		[Since (7,0), Mavericks]
		[Export ("beginActivityWithOptions:reason:")]
		NSObject BeginActivity (NSActivityOptions options, string reason);

		[Since (7,0), Mavericks]
		[Export ("endActivity:")]
		void EndActivity (NSObject activity);

		[Since (7,0), Mavericks]
		[Export ("performActivityWithOptions:reason:usingBlock:")]
		void PerformActivity (NSActivityOptions options, string reason, NSAction runCode);

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("isOperatingSystemAtLeastVersion:")]
		bool IsOperatingSystemAtLeastVersion (NSOperatingSystemVersion version);

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("operatingSystemVersion")]
		NSOperatingSystemVersion OperatingSystemVersion { get; }
		
#if MONOMAC
		[Export ("enableSuddenTermination")]
		void EnableSuddenTermination  ();

		[Export ("disableSuddenTermination")]
		void DisableSuddenTermination ();

		[Export ("enableAutomaticTermination:")]
		void EnableAutomaticTermination (string reason);

		[Export ("disableAutomaticTermination:")]
		void DisableAutomaticTermination (string reason);

		[Export ("automaticTerminationSupportEnabled")]
		bool AutomaticTerminationSupportEnabled { get; set; }
#else
		[Since (8,2)]
		[Export ("performExpiringActivityWithReason:usingBlock:")]
		void PerformExpiringActivity (string reason, Action<bool> block);

		[iOS (9,0)]
		[Export ("lowPowerModeEnabled")]
		bool LowPowerModeEnabled { [Bind ("isLowPowerModeEnabled")] get; }

		[iOS (9,0)]
		[Notification]
		[Field ("NSProcessInfoPowerStateDidChangeNotification")]
		NSString PowerStateDidChangeNotification { get; }
#endif

#if MONOMAC
		[Mac (10,10,3)]
		[Export ("thermalState")]
		NSProcessInfoThermalState ThermalState { get; }

		[Mac (10,10,3)]
		[Field ("NSProcessInfoThermalStateDidChangeNotification")]
		[Notification]
		NSString ThermalStateDidChangeNotification { get; }
#endif
	}

	[Since (7,0), Mavericks]
	[BaseType (typeof (NSObject))]
	public partial interface NSProgress {
	
		[Static, Export ("currentProgress")]
		NSProgress CurrentProgress { get; }
	
		[Static, Export ("progressWithTotalUnitCount:")]
		NSProgress FromTotalUnitCount (long unitCount);

		[iOS (9,0), Mac (10,11)]
		[Static, Export ("discreteProgressWithTotalUnitCount:")]
		NSProgress GetDiscreteProgress (long unitCount);

		[iOS (9,0), Mac (10,11)]
		[Static, Export ("progressWithTotalUnitCount:parent:pendingUnitCount:")]
		NSProgress FromTotalUnitCount (long unitCount, NSProgress parent, long portionOfParentTotalUnitCount);
	
		[DesignatedInitializer]
		[Export ("initWithParent:userInfo:")]
		IntPtr Constructor ([NullAllowed] NSProgress parentProgress, [NullAllowed] NSDictionary userInfo);
	
		[Export ("becomeCurrentWithPendingUnitCount:")]
		void BecomeCurrent (long pendingUnitCount);
	
		[Export ("resignCurrent")]
		void ResignCurrent ();
	
		[iOS (9,0), Mac (10,11)]
		[Export ("addChild:withPendingUnitCount:")]
		void AddChild (NSProgress child, long pendingUnitCount);

		[Export ("totalUnitCount")]
		long TotalUnitCount { get; set; }
	
		[Export ("completedUnitCount")]
		long CompletedUnitCount { get; set; }
	
		[Export ("localizedDescription", ArgumentSemantic.Copy), NullAllowed]
		string LocalizedDescription { get; set; }
	
		[Export ("localizedAdditionalDescription", ArgumentSemantic.Copy), NullAllowed]
		string LocalizedAdditionalDescription { get; set; }
	
		[Export ("cancellable")]
		bool Cancellable { [Bind ("isCancellable")] get; set; }
	
		[Export ("pausable")]
		bool Pausable { [Bind ("isPausable")] get; set; }
	
		[Export ("cancelled")]
		bool Cancelled { [Bind ("isCancelled")] get; }
	
		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; }
	
		[Export ("setCancellationHandler:")]
		void SetCancellationHandler (NSAction handler);
	
		[Export ("setPausingHandler:")]
		void SetPauseHandler (NSAction handler);

		[iOS (9,0), Mac (10,11)]
		[Export ("setResumingHandler:")]
		void SetResumingHandler (NSAction handler);
	
		[Export ("setUserInfoObject:forKey:")]
		void SetUserInfo ([NullAllowed] NSObject obj, NSString key);
	
		[Export ("indeterminate")]
		bool Indeterminate { [Bind ("isIndeterminate")] get; }
	
		[Export ("fractionCompleted")]
		double FractionCompleted { get; }
	
		[Export ("cancel")]
		void Cancel ();
	
		[Export ("pause")]
		void Pause ();

		[iOS (9,0), Mac (10,11)]
		[Export ("resume")]
		void Resume ();
	
		[Export ("userInfo")]
		NSDictionary UserInfo { get; }
	
		[NullAllowed] // by default this property is null
		[Export ("kind", ArgumentSemantic.Copy)]
		NSString Kind { get; set; }

#if MONOMAC
		[Export ("publish")]
		void Publish ();
	
		[Export ("unpublish")]
		void Unpublish ();
	
		[Export ("setAcknowledgementHandler:forAppBundleIdentifier:")]
		void SetAcknowledgementHandler (Action<bool> acknowledgementHandler, string appBundleIdentifier);
	
		[Static, Export ("addSubscriberForFileURL:withPublishingHandler:")]
		NSObject AddSubscriberForFile (NSUrl url, Action<NSProgress> publishingHandler);
	
		[Static, Export ("removeSubscriber:")]
		void RemoveSubscriber (NSObject subscriber);
	
		[Export ("acknowledgeWithSuccess:")]
		void AcknowledgeWithSuccess (bool success);
	
		[Export ("old")]
		bool Old { [Bind ("isOld")] get; }
#endif
		[Since (7,0), Field ("NSProgressKindFile")]
		NSString KindFile { get; }
	
		[Since (7,0), Field ("NSProgressEstimatedTimeRemainingKey")]
		NSString EstimatedTimeRemainingKey { get; }
	
		[Since (7,0), Field ("NSProgressThroughputKey")]
		NSString ThroughputKey { get; }
	
		[Since (7,0), Field ("NSProgressFileOperationKindKey")]
		NSString FileOperationKindKey { get; }
	
		[Since (7,0), Field ("NSProgressFileOperationKindDownloading")]
		NSString FileOperationKindDownloading { get; }
	
		[Since (7,0), Field ("NSProgressFileOperationKindDecompressingAfterDownloading")]
		NSString FileOperationKindDecompressingAfterDownloading { get; }
	
		[Since (7,0), Field ("NSProgressFileOperationKindReceiving")]
		NSString FileOperationKindReceiving { get; }
	
		[Since (7,0), Field ("NSProgressFileOperationKindCopying")]
		NSString FileOperationKindCopying { get; }
	
		[Since (7,0), Field ("NSProgressFileURLKey")]
		NSString FileURLKey { get; }
	
		[Since (7,0), Field ("NSProgressFileTotalCountKey")]
		NSString FileTotalCountKey { get; }
	
		[Since (7,0), Field ("NSProgressFileCompletedCountKey")]
		NSString FileCompletedCountKey { get; }

#if MONOMAC
		[Field ("NSProgressFileAnimationImageKey")]
		NSString FileAnimationImageKey { get; }
	
		[Field ("NSProgressFileAnimationImageOriginalRectKey")]
		NSString FileAnimationImageOriginalRectKey { get; }
	
		[Field ("NSProgressFileIconKey")]
		NSString FileIconKey { get; }
#endif
	}

	interface INSProgressReporting {}

	[iOS (9,0)][Mac (10,11)]
	[Protocol]
	interface NSProgressReporting {
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("progress")]
		NSProgress Progress { get; }
	}
	
	[BaseType (typeof (NSMutableData))]
	[Since (4,0)]
	public interface NSPurgeableData : NSSecureCoding, NSMutableCopying, NSDiscardableContent {
	}

	[Protocol]
	public interface NSDiscardableContent {
		[Abstract]
		[Export ("beginContentAccess")]
		bool BeginContentAccess ();

		[Abstract]
		[Export ("endContentAccess")]
		void EndContentAccess ();

		[Abstract]
		[Export ("discardContentIfPossible")]
		void DiscardContentIfPossible ();

		[Abstract]
		[Export ("isContentDiscarded")]
		bool IsContentDiscarded { get; }
	}

#if !XAMCORE_2_0
	public delegate void NSFileCoordinatorWorker (NSUrl newUrl);
#endif
	public delegate void NSFileCoordinatorWorkerRW (NSUrl newReadingUrl, NSUrl newWritingUrl);

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	interface NSFileCoordinator {
		[Static, Export ("addFilePresenter:")][PostGet ("FilePresenters")]
		void AddFilePresenter ([Protocolize] NSFilePresenter filePresenter);

		[Static]
		[Export ("removeFilePresenter:")][PostGet ("FilePresenters")]
		void RemoveFilePresenter ([Protocolize] NSFilePresenter filePresenter);

		[Static]
		[Export ("filePresenters")]
		[Protocolize]
		NSFilePresenter [] FilePresenters { get; }

		[DesignatedInitializer]
		[Export ("initWithFilePresenter:")]
		IntPtr Constructor ([NullAllowed] NSFilePresenter filePresenterOrNil);

		[Export ("coordinateReadingItemAtURL:options:error:byAccessor:")]
#if XAMCORE_2_0
		void CoordinateRead (NSUrl itemUrl, NSFileCoordinatorReadingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);
#else
		void CoordinateRead (NSUrl itemUrl, NSFileCoordinatorReadingOptions options, out NSError error, /* non null */ NSFileCoordinatorWorker worker);
#endif

		[Export ("coordinateWritingItemAtURL:options:error:byAccessor:")]
#if XAMCORE_2_0
		void CoordinateWrite (NSUrl url, NSFileCoordinatorWritingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);
#else
		void CoordinateWrite (NSUrl url, NSFileCoordinatorWritingOptions options, out NSError error, /* non null */ NSFileCoordinatorWorker worker);
#endif

		[Export ("coordinateReadingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateReadWrite (NSUrl readingURL, NSFileCoordinatorReadingOptions readingOptions, NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ NSFileCoordinatorWorkerRW readWriteWorker);
		
		[Export ("coordinateWritingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateWriteWrite (NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, NSUrl writingURL2, NSFileCoordinatorWritingOptions writingOptions2, out NSError error, /* non null */ NSFileCoordinatorWorkerRW writeWriteWorker);

		[Export ("prepareForReadingItemsAtURLs:options:writingItemsAtURLs:options:error:byAccessor:")]
		void CoordinateBatc (NSUrl [] readingURLs, NSFileCoordinatorReadingOptions readingOptions, NSUrl [] writingURLs, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ NSAction batchHandler);

		[iOS (8,0)][Mac (10,10)]
		[Export ("coordinateAccessWithIntents:queue:byAccessor:")]
		void CoordinateAccess (NSFileAccessIntent [] intents, NSOperationQueue executionQueue, Action<NSError> accessor);

		[Export ("itemAtURL:didMoveToURL:")]
		void ItemMoved (NSUrl fromUrl, NSUrl toUrl);

		[Export ("cancel")]
		void Cancel ();

		[Since (6,0)]
		[MountainLion]
		[Export ("itemAtURL:willMoveToURL:")]
		void WillMove (NSUrl oldUrl, NSUrl newUrl);

		[iOS (5,0)][Mac (10,7)]
		[Export ("purposeIdentifier")]
		string PurposeIdentifier { get; set; }
	}

	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSFileAccessIntent {
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Static, Export ("readingIntentWithURL:options:")]
		NSFileAccessIntent CreateReadingIntent (NSUrl url, NSFileCoordinatorReadingOptions options);

		[Static, Export ("writingIntentWithURL:options:")]
		NSFileAccessIntent CreateWritingIntent (NSUrl url, NSFileCoordinatorWritingOptions options);
	}
	
	[BaseType (typeof (NSObject))]
	public partial interface NSFileManager {
		[Field("NSFileType")]
		NSString NSFileType { get; }

		[Field("NSFileTypeDirectory")]
		NSString TypeDirectory { get; }

		[Field("NSFileTypeRegular")]
		NSString TypeRegular { get; }

		[Field("NSFileTypeSymbolicLink")]
		NSString TypeSymbolicLink { get; }

		[Field("NSFileTypeSocket")]
		NSString TypeSocket { get; }

		[Field("NSFileTypeCharacterSpecial")]
		NSString TypeCharacterSpecial { get; }

		[Field("NSFileTypeBlockSpecial")]
		NSString TypeBlockSpecial { get; }

		[Field("NSFileTypeUnknown")]
		NSString TypeUnknown { get; }

		[Field("NSFileSize")]
		NSString Size { get; }

		[Field("NSFileModificationDate")]
		NSString ModificationDate { get; }

		[Field("NSFileReferenceCount")]
		NSString ReferenceCount { get; }

		[Field("NSFileDeviceIdentifier")]
		NSString DeviceIdentifier { get; }

		[Field("NSFileOwnerAccountName")]
		NSString OwnerAccountName { get; }

		[Field("NSFileGroupOwnerAccountName")]
		NSString GroupOwnerAccountName { get; }

		[Field("NSFilePosixPermissions")]
		NSString PosixPermissions { get; }

		[Field("NSFileSystemNumber")]
		NSString SystemNumber { get; }

		[Field("NSFileSystemFileNumber")]
		NSString SystemFileNumber { get; }

		[Field("NSFileExtensionHidden")]
		NSString ExtensionHidden { get; }

		[Field("NSFileHFSCreatorCode")]
		NSString HfsCreatorCode { get; }

		[Field("NSFileHFSTypeCode")]
		NSString HfsTypeCode { get; }

		[Field("NSFileImmutable")]
		NSString Immutable { get; }

		[Field("NSFileAppendOnly")]
		NSString AppendOnly { get; }

		[Field("NSFileCreationDate")]
		NSString CreationDate { get; }

		[Field("NSFileOwnerAccountID")]
		NSString OwnerAccountID { get; }

		[Field("NSFileGroupOwnerAccountID")]
		NSString GroupOwnerAccountID { get; }

		[Field("NSFileBusy")]
		NSString Busy { get; }

#if !MONOMAC
		[iOS (4,0)]
		[Field ("NSFileProtectionKey")]
		NSString FileProtectionKey { get; }

		[iOS (4,0)]
		[Field ("NSFileProtectionNone")]
		NSString FileProtectionNone { get; }

		[iOS (4,0)]
		[Field ("NSFileProtectionComplete")]
		NSString FileProtectionComplete { get; }

		[Since (5,0)]
		[Field ("NSFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		[Since (5,0)]
		[Field ("NSFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication  { get; }
#endif
		[Field("NSFileSystemSize")]
		NSString SystemSize { get; }

		[Field("NSFileSystemFreeSize")]
		NSString SystemFreeSize { get; }

		[Field("NSFileSystemNodes")]
		NSString SystemNodes { get; }

		[Field("NSFileSystemFreeNodes")]
		NSString SystemFreeNodes { get; }

		[Static, Export ("defaultManager")]
		NSFileManager DefaultManager { get; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSFileManagerDelegate Delegate { get; set; }

		[Export ("setAttributes:ofItemAtPath:error:")]
		bool SetAttributes (NSDictionary attributes, string path, out NSError error);

		[Export ("createDirectoryAtPath:withIntermediateDirectories:attributes:error:")]
		bool CreateDirectory (string path, bool createIntermediates, [NullAllowed] NSDictionary attributes, out NSError error);

		[Export ("contentsOfDirectoryAtPath:error:")]
		string[] GetDirectoryContent (string path, out NSError error);

		[Export ("subpathsOfDirectoryAtPath:error:")]
		string[] GetDirectoryContentRecursive (string path, out NSError error);

		[Export ("attributesOfItemAtPath:error:")][Internal]
		NSDictionary _GetAttributes (string path, out NSError error);

		[Export ("attributesOfFileSystemForPath:error:")][Internal]
		NSDictionary _GetFileSystemAttributes (String path, out NSError error);

		[Export ("createSymbolicLinkAtPath:withDestinationPath:error:")]
		bool CreateSymbolicLink (string path, string destPath, out NSError error);

		[Export ("destinationOfSymbolicLinkAtPath:error:")]
		string GetSymbolicLinkDestination (string path, out NSError error);

		[Export ("copyItemAtPath:toPath:error:")]
		bool Copy (string srcPath, string dstPath, out NSError error);

		[Export ("moveItemAtPath:toPath:error:")]
		bool Move (string srcPath, string dstPath, out NSError error);

		[Export ("linkItemAtPath:toPath:error:")]
		bool Link (string srcPath, string dstPath, out NSError error);

		[Export ("removeItemAtPath:error:")]
		bool Remove ([NullAllowed] string path, out NSError error);

#if DEPRECATED
		// These are not available on iOS, and deprecated on OSX.
		[Export ("linkPath:toPath:handler:")]
		bool LinkPath (string src, string dest, IntPtr handler);

		[Export ("copyPath:toPath:handler:")]
		bool CopyPath (string src, string dest, IntPtr handler);

		[Export ("movePath:toPath:handler:")]
		bool MovePath (string src, string dest, IntPtr handler);

		[Export ("removeFileAtPath:handler:")]
		bool RemoveFileAtPath (string path, IntPtr handler);
#endif
		[Export ("currentDirectoryPath")]
		string GetCurrentDirectory ();

		[Export ("changeCurrentDirectoryPath:")]
		bool ChangeCurrentDirectory (string path);

		[Export ("fileExistsAtPath:")]
		bool FileExists (string path);

		[Export ("fileExistsAtPath:isDirectory:")]
		bool FileExists (string path, ref bool isDirectory);

		[Export ("isReadableFileAtPath:")]
		bool IsReadableFile (string path);

		[Export ("isWritableFileAtPath:")]
		bool IsWritableFile (string path);

		[Export ("isExecutableFileAtPath:")]
		bool IsExecutableFile (string path);

		[Export ("isDeletableFileAtPath:")]
		bool IsDeletableFile (string path);

		[Export ("contentsEqualAtPath:andPath:")]
		bool ContentsEqual (string path1, string path2);

		[Export ("displayNameAtPath:")]
		string DisplayName (string path);

		[Export ("componentsToDisplayForPath:")]
		string[] ComponentsToDisplay (string path);

		[Export ("enumeratorAtPath:")]
		NSDirectoryEnumerator GetEnumerator (string path);

		[Export ("subpathsAtPath:")]
		string[] Subpaths (string path);

		[Export ("contentsAtPath:")]
		NSData Contents (string path);

		[Export ("createFileAtPath:contents:attributes:")]
		bool CreateFile (string path, NSData data, [NullAllowed] NSDictionary attr);

		[Since (4,0)]
		[Export ("contentsOfDirectoryAtURL:includingPropertiesForKeys:options:error:")]
		NSUrl[] GetDirectoryContent (NSUrl url, [NullAllowed] NSArray properties, NSDirectoryEnumerationOptions options, out NSError error);

		[Since (4,0)]
		[Export ("copyItemAtURL:toURL:error:")]
		bool Copy (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Since (4,0)]
		[Export ("moveItemAtURL:toURL:error:")]
		bool Move (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Since (4,0)]
		[Export ("linkItemAtURL:toURL:error:")]
		bool Link (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Since (4,0)]
		[Export ("removeItemAtURL:error:")]
		bool Remove ([NullAllowed] NSUrl url, out NSError error);

		[Since (4,0)]
		[Export ("enumeratorAtURL:includingPropertiesForKeys:options:errorHandler:")]
#if XAMCORE_2_0
		NSDirectoryEnumerator GetEnumerator (NSUrl url, [NullAllowed] NSString[] keys, NSDirectoryEnumerationOptions options, [NullAllowed] NSEnumerateErrorHandler handler);
#else
		NSDirectoryEnumerator GetEnumerator (NSUrl url, [NullAllowed] NSArray properties, NSDirectoryEnumerationOptions options, [NullAllowed] NSEnumerateErrorHandler handler);
#endif

		[Since (4,0)]
		[Export ("URLForDirectory:inDomain:appropriateForURL:create:error:")]
		NSUrl GetUrl (NSSearchPathDirectory directory, NSSearchPathDomain domain, [NullAllowed] NSUrl url, bool shouldCreate, out NSError error);

		[Since (4,0)]
		[Export ("URLsForDirectory:inDomains:")]
		NSUrl[] GetUrls (NSSearchPathDirectory directory, NSSearchPathDomain domains);

		[Since (4,0)]
		[Export ("replaceItemAtURL:withItemAtURL:backupItemName:options:resultingItemURL:error:")]
		bool Replace (NSUrl originalItem, NSUrl newItem, [NullAllowed] string backupItemName, NSFileManagerItemReplacementOptions options, out NSUrl resultingURL, out NSError error);

		[Since (4,0)]
		[Export ("mountedVolumeURLsIncludingResourceValuesForKeys:options:")]
		NSUrl[] GetMountedVolumes ([NullAllowed] NSArray properties, NSVolumeEnumerationOptions options);

		// Methods to convert paths to/from C strings for passing to system calls - Not implemented
		////- (const char *)fileSystemRepresentationWithPath:(NSString *)path;
		//[Export ("fileSystemRepresentationWithPath:")]
		//const char FileSystemRepresentationWithPath (string path);

		////- (NSString *)stringWithFileSystemRepresentation:(const char *)str length:(NSUInteger)len;
		//[Export ("stringWithFileSystemRepresentation:length:")]
		//string StringWithFileSystemRepresentation (const char str, uint len);

		[Since (5,0)]
		[Export ("createDirectoryAtURL:withIntermediateDirectories:attributes:error:")]
		bool CreateDirectory (NSUrl url, bool createIntermediates, [NullAllowed] NSDictionary attributes, out NSError error);

		[Since (5,0)]
                [Export ("createSymbolicLinkAtURL:withDestinationURL:error:")]
                bool CreateSymbolicLink (NSUrl url, NSUrl destURL, out NSError error);

		[Since (5,0)]
                [Export ("setUbiquitous:itemAtURL:destinationURL:error:")]
                bool SetUbiquitous (bool flag, NSUrl url, NSUrl destinationUrl, out NSError error);

		[Since (5,0)]
                [Export ("isUbiquitousItemAtURL:")]
                bool IsUbiquitous (NSUrl url);

		[Since (5,0)]
                [Export ("startDownloadingUbiquitousItemAtURL:error:")]
                bool StartDownloadingUbiquitous (NSUrl url, out NSError error);

		[Since (5,0)]
                [Export ("evictUbiquitousItemAtURL:error:")]
                bool EvictUbiquitous (NSUrl url, out NSError error);

		[Since (5,0)]
                [Export ("URLForUbiquityContainerIdentifier:")]
                NSUrl GetUrlForUbiquityContainer ([NullAllowed] string containerIdentifier);

		[Since (5,0)]
                [Export ("URLForPublishingUbiquitousItemAtURL:expirationDate:error:")]
                NSUrl GetUrlForPublishingUbiquitousItem (NSUrl url, out NSDate expirationDate, out NSError error);

		[Since (6,0)]
		[MountainLion]
		[Export ("ubiquityIdentityToken")]
		NSObject UbiquityIdentityToken { get; }

		[Since (6,0)]
		[MountainLion]
		[Field ("NSUbiquityIdentityDidChangeNotification")]
		[Notification]
		NSString UbiquityIdentityDidChangeNotification { get; }

		[Since (7,0), MountainLion]
		[Export ("containerURLForSecurityApplicationGroupIdentifier:")]
		NSUrl GetContainerUrl (string securityApplicationGroupIdentifier);

		[iOS (8,0), Mac (10,10)]
		[Export ("getRelationship:ofDirectory:inDomain:toItemAtURL:error:")]
#if XAMCORE_2_0
		bool GetRelationship (out NSUrlRelationship outRelationship, NSSearchPathDirectory directory, NSSearchPathDomain domain, NSUrl toItemAtUrl, out NSError error);
#else
		bool GetRelationship (out NSURLRelationship outRelationship, NSSearchPathDirectory directory, NSSearchPathDomain domain, NSUrl toItemAtUrl, out NSError error);
#endif

		[iOS (8,0), Mac (10,10)]
		[Export ("getRelationship:ofDirectoryAtURL:toItemAtURL:error:")]
#if XAMCORE_2_0
		bool GetRelationship (out NSUrlRelationship outRelationship, NSUrl directoryURL, NSUrl otherURL, out NSError error);
#else
		bool GetRelationship (out NSURLRelationship outRelationship, NSUrl directoryURL, NSUrl otherURL, out NSError error);
#endif
	}

	[BaseType(typeof(NSObject))]
	[Model]
	[Protocol]
	public interface NSFileManagerDelegate {
		[Export("fileManager:shouldCopyItemAtPath:toPath:")]
		bool ShouldCopyItemAtPath(NSFileManager fm, NSString srcPath, NSString dstPath);

#if !MONOMAC
		[Export("fileManager:shouldCopyItemAtURL:toURL:")]
		bool ShouldCopyItemAtUrl(NSFileManager fm, NSUrl srcUrl, NSUrl dstUrl);
		
		[Export ("fileManager:shouldLinkItemAtURL:toURL:")]
		bool ShouldLinkItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldMoveItemAtURL:toURL:")]
		bool ShouldMoveItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldProceedAfterError:copyingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorCopyingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldProceedAfterError:linkingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorLinkingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldProceedAfterError:movingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorMovingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldRemoveItemAtURL:")]
		bool ShouldRemoveItemAtUrl (NSFileManager fileManager, NSUrl url);

		[Export ("fileManager:shouldProceedAfterError:removingItemAtURL:")]
		bool ShouldProceedAfterErrorRemovingItem (NSFileManager fileManager, NSError error, NSUrl url);
#endif

		[Export ("fileManager:shouldProceedAfterError:copyingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorCopyingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		[Export ("fileManager:shouldMoveItemAtPath:toPath:")]
		bool ShouldMoveItemAtPath (NSFileManager fileManager, string srcPath, string dstPath);

		[Export ("fileManager:shouldProceedAfterError:movingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorMovingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		[Export ("fileManager:shouldLinkItemAtPath:toPath:")]
		bool ShouldLinkItemAtPath (NSFileManager fileManager, string srcPath, string dstPath);

		[Export ("fileManager:shouldProceedAfterError:linkingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorLinkingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		[Export ("fileManager:shouldRemoveItemAtPath:")]
		bool ShouldRemoveItemAtPath (NSFileManager fileManager, string path);

		[Export ("fileManager:shouldProceedAfterError:removingItemAtPath:")]
		bool ShouldProceedAfterErrorRemovingItem (NSFileManager fileManager, NSError error, string path);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSFilePresenter {
		[Abstract]
		[Export ("presentedItemURL", ArgumentSemantic.Retain)]
		NSUrl PresentedItemURL { get; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("presentedItemOperationQueue", ArgumentSemantic.Retain)]
		NSOperationQueue PesentedItemOperationQueue { get; }

#if DOUBLE_BLOCKS
		[Export ("relinquishPresentedItemToReader:")]
		void RelinquishPresentedItemToReader (NSFilePresenterReacquirer readerAction);

		[Export ("relinquishPresentedItemToWriter:")]
		void RelinquishPresentedItemToWriter (NSFilePresenterReacquirer writerAction);
#endif

		[Export ("savePresentedItemChangesWithCompletionHandler:")]
		void SavePresentedItemChanges (Action<NSError> completionHandler);

		[Export ("accommodatePresentedItemDeletionWithCompletionHandler:")]
		void AccommodatePresentedItemDeletion (Action<NSError> completionHandler);

		[Export ("presentedItemDidMoveToURL:")]
		void PresentedItemMoved (NSUrl newURL);

		[Export ("presentedItemDidChange")]
		void PresentedItemChanged ();

		[Export ("presentedItemDidGainVersion:")]
		void PresentedItemGainedVersion (NSFileVersion version);

		[Export ("presentedItemDidLoseVersion:")]
		void PresentedItemLostVersion (NSFileVersion version);

		[Export ("presentedItemDidResolveConflictVersion:")]
		void PresentedItemResolveConflictVersion (NSFileVersion version);

		[Export ("accommodatePresentedSubitemDeletionAtURL:completionHandler:")]
		void AccommodatePresentedSubitemDeletion (NSUrl url, Action<NSError> completionHandler);

		[Export ("presentedSubitemDidAppearAtURL:")]
		void PresentedSubitemAppeared (NSUrl atUrl);

		[Export ("presentedSubitemAtURL:didMoveToURL:")]
		void PresentedSubitemMoved (NSUrl oldURL, NSUrl newURL);

		[Export ("presentedSubitemDidChangeAtURL:")]
		void PresentedSubitemChanged (NSUrl url);

		[Export ("presentedSubitemAtURL:didGainVersion:")]
		void PresentedSubitemGainedVersion (NSUrl url, NSFileVersion version);

		[Export ("presentedSubitemAtURL:didLoseVersion:")]
		void PresentedSubitemLostVersion (NSUrl url, NSFileVersion version);

		[Export ("presentedSubitemAtURL:didResolveConflictVersion:")]
		void PresentedSubitemResolvedConflictVersion (NSUrl url, NSFileVersion version);
	}

	delegate void NSFileVersionNonlocalVersionsCompletionHandler ([NullAllowed] NSFileVersion[] nonlocalFileVersions, [NullAllowed] NSError error);

	[Since (5,0)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[NSFileVersion init]: You have to use one of the factory methods to instantiate NSFileVersion.
	[DisableDefaultCtor]
	interface NSFileVersion {
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get;  }

		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get;  }

		[Export ("localizedNameOfSavingComputer", ArgumentSemantic.Copy)]
		string LocalizedNameOfSavingComputer { get;  }

		[Export ("modificationDate", ArgumentSemantic.Copy)]
		NSDate ModificationDate { get;  }

		[Export ("persistentIdentifier", ArgumentSemantic.Retain)]
		NSObject PersistentIdentifier { get;  }

		[Export ("conflict")]
		bool IsConflict { [Bind ("isConflict")] get;  }

		[Export ("resolved")]
		bool Resolved { [Bind ("isResolved")] get; set;  }
#if MONOMAC
		[Export ("discardable")]
		bool Discardable { [Bind ("isDiscardable")] get; set;  }
#endif

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("hasLocalContents")]
		bool HasLocalContents { get; }

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("hasThumbnail")]
		bool HasThumbnail { get; }

		[Static]
		[Export ("currentVersionOfItemAtURL:")]
		NSFileVersion GetCurrentVersion (NSUrl url);

		[Mac (10,10)]
		[iOS (8,0)]
		[Static]
		[Export ("getNonlocalVersionsOfItemAtURL:completionHandler:")]
		void GetNonlocalVersions (NSUrl url, NSFileVersionNonlocalVersionsCompletionHandler completionHandler);

		[Static]
		[Export ("otherVersionsOfItemAtURL:")]
		NSFileVersion [] GetOtherVersions (NSUrl url);

		[Static]
		[Export ("unresolvedConflictVersionsOfItemAtURL:")]
		NSFileVersion [] GetUnresolvedConflictVersions (NSUrl url);

		[Static]
		[Export ("versionOfItemAtURL:forPersistentIdentifier:")]
		NSFileVersion GetSpecificVersion (NSUrl url, NSObject persistentIdentifier);

#if MONOMAC
		[Static]
		[Export ("addVersionOfItemAtURL:withContentsOfURL:options:error:")]
		NSFileVersion AddVersion (NSUrl url, NSUrl contentsURL, NSFileVersionAddingOptions options, out NSError outError);

		[Static]
		[Export ("temporaryDirectoryURLForNewVersionOfItemAtURL:")]
		NSUrl TemporaryDirectoryForItem (NSUrl url);
#endif

		[Export ("replaceItemAtURL:options:error:")]
		NSUrl ReplaceItem (NSUrl url, NSFileVersionReplacingOptions options, out NSError error);

		[Export ("removeAndReturnError:")]
		bool Remove (out NSError outError);

		[Static]
		[Export ("removeOtherVersionsOfItemAtURL:error:")]
		bool RemoveOtherVersions (NSUrl url, out NSError outError);
	}

	[BaseType (typeof (NSObject))]
	public interface NSFileWrapper : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithURL:options:error:")]
		IntPtr Constructor (NSUrl url, NSFileWrapperReadingOptions options, out NSError outError);

		[DesignatedInitializer]
		[Export ("initDirectoryWithFileWrappers:")]
		IntPtr Constructor (NSDictionary childrenByPreferredName);

		[DesignatedInitializer]
		[Export ("initRegularFileWithContents:")]
		IntPtr Constructor (NSData contents);

		[DesignatedInitializer]
		[Export ("initSymbolicLinkWithDestinationURL:")]
		IntPtr Constructor (NSUrl urlToSymbolicLink);

		// Constructor clash
		//[Export ("initWithSerializedRepresentation:")]
		//IntPtr Constructor (NSData serializeRepresentation);

		[Export ("isDirectory")]
		bool IsDirectory { get; }

		[Export ("isRegularFile")]
		bool IsRegularFile { get; }

		[Export ("isSymbolicLink")]
		bool IsSymbolicLink { get; }

		[Export ("matchesContentsOfURL:")]
		bool MatchesContentsOfURL (NSUrl url);

		[Export ("readFromURL:options:error:")]
		bool Read (NSUrl url, NSFileWrapperReadingOptions options, out NSError outError);

		[Export ("writeToURL:options:originalContentsURL:error:")]
		bool Write (NSUrl url, NSFileWrapperWritingOptions options, NSUrl originalContentsURL, out NSError outError);

		[Export ("serializedRepresentation")]
		NSData GetSerializedRepresentation ();

		[Export ("addFileWrapper:")]
		string AddFileWrapper (NSFileWrapper child);

		[Export ("addRegularFileWithContents:preferredFilename:")]
		string AddRegularFile (NSData dataContents, string preferredFilename);

		[Export ("removeFileWrapper:")]
		void RemoveFileWrapper (NSFileWrapper child);

		[Export ("fileWrappers")]
		NSDictionary FileWrappers { get; }

		[Export ("keyForFileWrapper:")]
		string KeyForFileWrapper (NSFileWrapper child);

		[Export ("regularFileContents")]
		NSData GetRegularFileContents ();

		[Export ("symbolicLinkDestinationURL")]
		NSUrl SymbolicLinkDestinationURL { get; }

		//Detected properties
		// [NullAllowed] can't be used. It's null by default but, on device, it throws-n-crash
		// NSInvalidArgumentException -[NSFileWrapper setPreferredFilename:] *** preferredFilename cannot be empty.
		[Export ("preferredFilename")]
		string PreferredFilename { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("filename")]
		string Filename { get; set; }

		[Export ("fileAttributes", ArgumentSemantic.Copy)]
		NSDictionary FileAttributes { get; set; }

#if MONOMAC
		[Export ("icon", ArgumentSemantic.Retain)]
		NSImage Icon { get; set; }
#endif
	}

	[BaseType (typeof (NSEnumerator))]
	public interface NSDirectoryEnumerator {
		[Export ("fileAttributes")]
		NSDictionary FileAttributes { get; }

		[Export ("directoryAttributes")]
		NSDictionary DirectoryAttributes { get; }

		[Export ("skipDescendents")]
		void SkipDescendents ();

#if !MONOMAC
		[Export ("level")]
		nint Level { get; }
#else
#if !XAMCORE_2_0
		////- (unsigned long long)fileSize;
		//[Export ("fileSize")]
		//unsigned long long FileSize ([Target] NSDictionary fileAttributes);

		[Bind ("fileModificationDate")]
		[Obsolete ("Use ToFileAttributes ().ModificationDate instead")]
		NSDate FileModificationDate ([Target] NSDictionary fileAttributes);

		[Bind ("fileType")]
		[Obsolete ("Use ToFileAttributes ().Type instead")]
		string FileType ([Target] NSDictionary fileAttributes);

		[Bind ("filePosixPermissions")]
		[Obsolete ("Use ToFileAttributes ().PosixPermissions instead")]
		uint /* unsigned short */ FilePosixPermissions ([Target] NSDictionary fileAttributes);

		[Bind ("fileOwnerAccountName")]
		[Obsolete ("Use ToFileAttributes ().OwnerAccountName instead")]
		string FileOwnerAccountName ([Target] NSDictionary fileAttributes);

		[Bind ("fileGroupOwnerAccountName")]
		[Obsolete ("Use ToFileAttributes ().GroupOwnerAccountName instead")]
		string FileGroupOwnerAccountName ([Target] NSDictionary fileAttributes);

		[Bind ("fileSystemNumber")]
		[Obsolete ("Use ToFileAttributes ().SystemNumber instead")]
		nint FileSystemNumber ([Target] NSDictionary fileAttributes);

		[Bind ("fileSystemFileNumber")]
		[Obsolete ("Use ToFileAttributes ().SystemFileNumber instead")]
		nuint FileSystemFileNumber ([Target] NSDictionary fileAttributes);

		[Bind ("fileExtensionHidden")]
		[Obsolete ("Use ToFileAttributes ().ExtensionHidden instead")]
		bool FileExtensionHidden ([Target] NSDictionary fileAttributes);

		[Bind ("fileHFSCreatorCode")]
		[Obsolete ("Use ToFileAttributes ().HfsCreatorCode instead")]
		nuint FileHfsCreatorCode ([Target] NSDictionary fileAttributes);

		[Bind ("fileHFSTypeCode")]
		[Obsolete ("Use ToFileAttributes ().HfsTypeCode instead")]
		nuint FileHfsTypeCode ([Target] NSDictionary fileAttributes);

		[Bind ("fileIsImmutable")]
		[Obsolete ("Use ToFileAttributes ().IsImmutable instead")]
		bool FileIsImmutable ([Target] NSDictionary fileAttributes);

		[Bind ("fileIsAppendOnly")]
		[Obsolete ("Use ToFileAttributes ().IsAppendOnly instead")]
		bool FileIsAppendOnly ([Target] NSDictionary fileAttributes);

		[Bind ("fileCreationDate")]
		[Obsolete ("Use ToFileAttributes ().CreationDate instead")]
		NSDate FileCreationDate ([Target] NSDictionary fileAttributes);

		[Bind ("fileOwnerAccountID")]
		[Obsolete ("Use ToFileAttributes ().OwnerAccountID instead")]
		NSNumber FileOwnerAccountID ([Target] NSDictionary fileAttributes);

		[Bind ("fileGroupOwnerAccountID")]
		[Obsolete ("Use ToFileAttributes ().GroupOwnerAccountID instead")]
		NSNumber FileGroupOwnerAccountID ([Target] NSDictionary fileAttributes);
#endif
#endif
	}

	public delegate bool NSPredicateEvaluator (NSObject evaluatedObject, NSDictionary bindings);
	
	[BaseType (typeof (NSObject))]
	[Since (4,0)]
	// 'init' returns NIL
	[DisableDefaultCtor]
	public interface NSPredicate : NSSecureCoding, NSCopying {
		[Static]
		[Internal]
		[Export ("predicateWithFormat:argumentArray:")]
		NSPredicate _FromFormat (string predicateFormat, [NullAllowed] NSObject[] arguments);

		[Static, Export ("predicateWithValue:")]
		NSPredicate FromValue (bool value);

		[Static, Export ("predicateWithBlock:")]
		NSPredicate FromExpression (NSPredicateEvaluator evaluator);

		[Export ("predicateFormat")]
		string PredicateFormat { get; }

		[Export ("predicateWithSubstitutionVariables:")]
		NSPredicate PredicateWithSubstitutionVariables (NSDictionary substitutionVariables);

		[Export ("evaluateWithObject:")]
		bool EvaluateWithObject (NSObject obj);

		[Export ("evaluateWithObject:substitutionVariables:")]
		bool EvaluateWithObject (NSObject obj, NSDictionary substitutionVariables);
#if MONOMAC
		// 10.9+
		[Static]
		[Mavericks]
		[Export ("predicateFromMetadataQueryString:")]
		NSPredicate FromMetadataQueryString (string query);
#endif
		[Since (7,0), Mavericks]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();
	}

	[Category, BaseType (typeof (NSOrderedSet))]
	public partial interface NSPredicateSupport_NSOrderedSet {
		[Since (5,0)]
		[Export ("filteredOrderedSetUsingPredicate:")]
		NSOrderedSet FilterUsingPredicate (NSPredicate p);
	}
	
	[Category, BaseType (typeof (NSMutableOrderedSet))]
	public partial interface NSPredicateSupport_NSMutableOrderedSet {
		[Since (5,0)]
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate p);
	}

	[Category, BaseType (typeof (NSArray))]
	public partial interface NSPredicateSupport_NSArray {
		[Export ("filteredArrayUsingPredicate:")]
		NSArray FilterUsingPredicate (NSArray array);
	}

#pragma warning disable 618
	[Category, BaseType (typeof (NSMutableArray))]
#pragma warning restore 618
	public partial interface NSPredicateSupport_NSMutableArray {
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate predicate);
	}
	
	[Category, BaseType (typeof (NSSet))]
	public partial interface NSPredicateSupport_NSSet {
		[Export ("filteredSetUsingPredicate:")]
		NSSet FilterUsingPredicate (NSPredicate predicate);
	}

	[Category, BaseType (typeof (NSMutableSet))]
	public partial interface NSPredicateSupport_NSMutableSet {
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate predicate);
	}
	
#if MONOMAC
	[BaseType (typeof (NSObject), Name="NSURLDownload")]
	public interface NSUrlDownload {
		[Static, Export ("canResumeDownloadDecodedWithEncodingMIMEType:")]
		bool CanResumeDownloadDecodedWithEncodingMimeType (string mimeType);

		[Export ("initWithRequest:delegate:")]
		IntPtr Constructor (NSUrlRequest request, NSObject delegate1);

		[Export ("initWithResumeData:delegate:path:")]
		IntPtr Constructor (NSData resumeData, NSObject delegate1, string path);

		[Export ("cancel")]
		void Cancel ();

		[Export ("setDestination:allowOverwrite:")]
		void SetDestination (string path, bool allowOverwrite);

		[Export ("request")]
		NSUrlRequest Request { get; }

		[Export ("resumeData")]
		NSData ResumeData { get; }

		[Export ("deletesFileUponFailure")]
		bool DeletesFileUponFailure { get; set; }
	}

    	[BaseType (typeof (NSObject))]
    	[Model]
	[Protocol (Name = "NSURLDownloadDelegate")]
	public interface NSUrlDownloadDelegate {
		[Export ("downloadDidBegin:")]
		void DownloadBegan (NSUrlDownload download);

		[Export ("download:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlDownload download, NSUrlRequest request, NSUrlResponse redirectResponse);

		[Export ("download:didReceiveAuthenticationChallenge:")]
		void ReceivedAuthenticationChallenge (NSUrlDownload download, NSUrlAuthenticationChallenge challenge);

		[Export ("download:didCancelAuthenticationChallenge:")]
		void CanceledAuthenticationChallenge (NSUrlDownload download, NSUrlAuthenticationChallenge challenge);

		[Export ("download:didReceiveResponse:")]
		void ReceivedResponse (NSUrlDownload download, NSUrlResponse response);

		//- (void)download:(NSUrlDownload *)download willResumeWithResponse:(NSUrlResponse *)response fromByte:(long long)startingByte;
		[Export ("download:willResumeWithResponse:fromByte:")]
		void Resume (NSUrlDownload download, NSUrlResponse response, long startingByte);

		//- (void)download:(NSUrlDownload *)download didReceiveDataOfLength:(NSUInteger)length;
		[Export ("download:didReceiveDataOfLength:")]
		void ReceivedData (NSUrlDownload download, nuint length);

		[Export ("download:shouldDecodeSourceDataOfMIMEType:")]
		bool DecodeSourceData (NSUrlDownload download, string encodingType);

		[Export ("download:decideDestinationWithSuggestedFilename:")]
		void DecideDestination (NSUrlDownload download, string suggestedFilename);

		[Export ("download:didCreateDestination:")]
		void CreatedDestination (NSUrlDownload download, string path);

		[Export ("downloadDidFinish:")]
		void Finished (NSUrlDownload download);

		[Export ("download:didFailWithError:")]
		void FailedWithError(NSUrlDownload download, NSError error);
	}
#endif

#if XAMCORE_2_0 && !MONOMAC
	// Users are not supposed to implement the NSUrlProtocolClient protocol, they're 
	// only supposed to consume it. This is why there's no model for this protocol.
	[Protocol (Name = "NSURLProtocolClient")]
	interface NSUrlProtocolClient {
		[Abstract]
		[Export ("URLProtocol:wasRedirectedToRequest:redirectResponse:")]
		void Redirected (NSUrlProtocol protocol, NSUrlRequest redirectedToEequest, NSUrlResponse redirectResponse);

		[Abstract]
		[Export ("URLProtocol:cachedResponseIsValid:")]
		void CachedResponseIsValid (NSUrlProtocol protocol, NSCachedUrlResponse cachedResponse);

		[Abstract]
		[Export ("URLProtocol:didReceiveResponse:cacheStoragePolicy:")]
		void ReceivedResponse (NSUrlProtocol protocol, NSUrlResponse response, NSUrlCacheStoragePolicy policy);

		[Abstract]
		[Export ("URLProtocol:didLoadData:")]
		void DataLoaded (NSUrlProtocol protocol, NSData data);

		[Abstract]
		[Export ("URLProtocolDidFinishLoading:")]
		void FinishedLoading (NSUrlProtocol protocol);

		[Abstract]
		[Export ("URLProtocol:didFailWithError:")]
		void FailedWithError (NSUrlProtocol protocol, NSError error);

		[Abstract]
		[Export ("URLProtocol:didReceiveAuthenticationChallenge:")]
		void ReceivedAuthenticationChallenge (NSUrlProtocol protocol, NSUrlAuthenticationChallenge challenge);

		[Abstract]
		[Export ("URLProtocol:didCancelAuthenticationChallenge:")]
		void CancelledAuthenticationChallenge (NSUrlProtocol protocol, NSUrlAuthenticationChallenge challenge);
	}
#else
	interface NSUrlProtocolClient {}
#endif

	interface INSUrlProtocolClient {}

	[BaseType (typeof (NSObject),
		   Name="NSURLProtocol",
		   Delegates=new string [] {"WeakClient"})]
	interface NSUrlProtocol {
		[DesignatedInitializer]
		[Export ("initWithRequest:cachedResponse:client:")]
#if XAMCORE_2_0 && !MONOMAC
		IntPtr Constructor (NSUrlRequest request, [NullAllowed] NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client);
#else
		IntPtr Constructor (NSUrlRequest request, [NullAllowed] NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client);
#endif

#if XAMCORE_2_0 && !MONOMAC
		[Export ("client")]
		INSUrlProtocolClient Client { get; }
#else
		[Export ("client")]
		NSObject WeakClient { get; }
#endif

		[Export ("request")]
		NSUrlRequest Request { get; }

		[Export ("cachedResponse")]
		NSCachedUrlResponse CachedResponse { get; }

		[Static]
		[Export ("canInitWithRequest:")]
		bool CanInitWithRequest (NSUrlRequest request);

		[Static]
		[Export ("canonicalRequestForRequest:")]
		NSUrlRequest GetCanonicalRequest (NSUrlRequest forRequest);

		[Static]
		[Export ("requestIsCacheEquivalent:toRequest:")]
		bool IsRequestCacheEquivalent (NSUrlRequest first, NSUrlRequest second);

		[Export ("startLoading")]
		void StartLoading ();

		[Export ("stopLoading")]
		void StopLoading ();

		[Static]
		[Export ("propertyForKey:inRequest:")]
		NSObject GetProperty (string key, NSUrlRequest inRequest);

		[Static]
		[Export ("setProperty:forKey:inRequest:")]
		void SetProperty ([NullAllowed] NSObject value, string key, NSMutableUrlRequest inRequest);

		[Static]
		[Export ("removePropertyForKey:inRequest:")]
		void RemoveProperty (string propertyKey, NSMutableUrlRequest request);

		[Static]
		[Export ("registerClass:")]
		bool RegisterClass (Class protocolClass);

		[Static]
		[Export ("unregisterClass:")]
		void UnregisterClass (Class protocolClass);

		// Commented API are broken and we'll need to provide a workaround for them
		// https://trello.com/c/RthKXnyu/381-disabled-nsurlprotocol-api-reminder

		// * "task" does not answer and is not usable - maybe it only works if created from the new API ?!?
		//
		// * "canInitWithTask" can't be called as a .NET static method. The ObjC code uses the current type
		//    internally (which will always be NSURLProtocol in .NET never a subclass) and complains about it
		//    being abstract (which is true)
		//    -canInitWithRequest: cannot be sent to an abstract object of class NSURLProtocol: Create a concrete instance!

//		[iOS (8,0)]
//		[Export ("initWithTask:cachedResponse:client:")]
//#if XAMCORE_2_0
//		IntPtr Constructor (NSUrlSessionTask task, [NullAllowed] NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client);
//#else
//		IntPtr Constructor (NSUrlSessionTask task, [NullAllowed] NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client);
//#endif
//		[iOS (8,0)]
//		[Export ("task", ArgumentSemantic.Copy)]
//		NSUrlSessionTask Task { get; }
//
//		[iOS (8,0)]
//		[Static, Export ("canInitWithTask:")]
//		bool CanInitWithTask (NSUrlSessionTask task);
	}

	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	public interface NSPropertyListSerialization {
		[Static, Export ("dataWithPropertyList:format:options:error:")]
		NSData DataWithPropertyList (NSObject plist, NSPropertyListFormat format,
			NSPropertyListWriteOptions options, out NSError error);

		[Static, Export ("writePropertyList:toStream:format:options:error:")]
		nint WritePropertyList (NSObject plist, NSOutputStream stream, NSPropertyListFormat format,
			NSPropertyListWriteOptions options, out NSError error);

		[Static, Export ("propertyListWithData:options:format:error:")]
		NSObject PropertyListWithData (NSData data, NSPropertyListReadOptions options,
			ref NSPropertyListFormat format, out NSError error);

		[Static, Export ("propertyListWithStream:options:format:error:")]
		NSObject PropertyListWithStream (NSInputStream stream, NSPropertyListReadOptions options,
			ref NSPropertyListFormat format, out NSError error);

		[Static, Export ("propertyList:isValidForFormat:")]
		bool IsValidForFormat (NSObject plist, NSPropertyListFormat format);
	}

#if XAMCORE_2_0 || !MONOMAC
	public interface INSExtensionRequestHandling { }

	[iOS (8,0)][Mac (10,10)] // Not defined in 32-bit
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	public interface NSExtensionRequestHandling {
		[Abstract]
		[Mac (10,10, onlyOn64 : true)] 
		// @required - (void)beginRequestWithExtensionContext:(NSExtensionContext *)context;
		[Export ("beginRequestWithExtensionContext:")]
		void BeginRequestWithExtensionContext (NSExtensionContext context);
	}
#else
	[iOS (8,0)][Mac (10,10, onlyOn64:true)] // Not defined in 32-bit
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	public interface NSExtensionRequestHandling {
		[Abstract]
		// @required - (void)beginRequestWithExtensionContext:(NSExtensionContext *)context;
		[Export ("beginRequestWithExtensionContext:")]
		void BeginRequestWithExtensionContext (NSExtensionContext context);
	}
#endif

	[Protocol]
	public interface NSLocking {

		[Abstract]
		[Export ("lock")]
		void Lock ();

		[Abstract]
		[Export ("unlock")]
		void Unlock ();
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // An uncaught exception was raised: *** -range cannot be sent to an abstract object of class NSTextCheckingResult: Create a concrete instance!
	[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
	public interface NSTextCheckingResult : NSSecureCoding, NSCopying {
		[Export ("resultType")]
		NSTextCheckingType ResultType { get;  }

		[Export ("range")]
		NSRange Range { get;  }

		// From the NSTextCheckingResultOptional category on NSTextCheckingResult
		[Export ("orthography")]
		NSOrthography Orthography { get; }

		[Export ("grammarDetails")]
		string[] GrammarDetails { get; }

		[Export ("date")]
		NSDate Date { get; }

		[Export ("timeZone")]
		NSTimeZone TimeZone { get; }

		[Export ("duration")]
		double TimeInterval { get; }

		[Export ("components")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSDictionary WeakComponents { get; }

		[Wrap ("WeakComponents")]
		NSTextCheckingTransitComponents Components { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("replacementString")]
		string ReplacementString { get; }

		[Export ("alternativeStrings")]
		[Availability (Introduced = Platform.iOS_7_0 | Platform.Mac_10_9)]
		string [] AlternativeStrings { get; }

//		NSRegularExpression isn't bound
//		[Export ("regularExpression")]
//		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
//		NSRegularExpression RegularExpression { get; }

		[Export ("phoneNumber")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		string PhoneNumber { get; }

		[Export ("addressComponents")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSDictionary WeakAddressComponents { get; }

		[Wrap ("WeakAddressComponents")]
		NSTextCheckingAddressComponents AddressComponents { get; }

		[Export ("numberOfRanges")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		nuint NumberOfRanges { get; }

		[Export ("rangeAtIndex:")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		NSRange RangeAtIndex (nuint idx);

		[Export ("resultByAdjustingRangesWithOffset:")]
		[Availability (Introduced = Platform.iOS_5_0 | Platform.Mac_10_7)]
		NSTextCheckingResult ResultByAdjustingRanges (nint offset);

		// From the NSTextCheckingResultCreation category on NSTextCheckingResult

		[Static]
		[Export ("orthographyCheckingResultWithRange:orthography:")]
		NSTextCheckingResult OrthographyCheckingResult (NSRange range, NSOrthography ortography);

		[Static]
		[Export ("spellCheckingResultWithRange:")]
		NSTextCheckingResult SpellCheckingResult (NSRange range);

		[Static]
		[Export ("grammarCheckingResultWithRange:details:")]
		NSTextCheckingResult GrammarCheckingResult (NSRange range, string[] details);

		[Static]
		[Export ("dateCheckingResultWithRange:date:")]
		NSTextCheckingResult DateCheckingResult (NSRange range, NSDate date);

		[Static]
		[Export ("dateCheckingResultWithRange:date:timeZone:duration:")]
		NSTextCheckingResult DateCheckingResult (NSRange range, NSDate date, NSTimeZone timezone, double duration);

		[Static]
		[Export ("addressCheckingResultWithRange:components:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSTextCheckingResult AddressCheckingResult (NSRange range, NSDictionary components);

		[Static]
		[Wrap ("AddressCheckingResult (range, components != null ? components.Dictionary : null)")]
		NSTextCheckingResult AddressCheckingResult (NSRange range, NSTextCheckingAddressComponents components);

		[Static]
		[Export ("linkCheckingResultWithRange:URL:")]
		NSTextCheckingResult LinkCheckingResult (NSRange range, NSUrl url);

		[Static]
		[Export ("quoteCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult QuoteCheckingResult (NSRange range, NSString replacementString);

		[Static]
		[Export ("dashCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult DashCheckingResult (NSRange range, string replacementString);

		[Static]
		[Export ("replacementCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult ReplacementCheckingResult (NSRange range, string replacementString);

		[Static]
		[Export ("correctionCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult CorrectionCheckingResult (NSRange range, string replacementString);

		[Static]
		[Export ("correctionCheckingResultWithRange:replacementString:alternativeStrings:")]
		[Availability (Introduced = Platform.iOS_7_0 | Platform.Mac_10_9)]
		NSTextCheckingResult CorrectionCheckingResult (NSRange range, string replacementString, string[] alternativeStrings);

//		NSRegularExpression isn't bound
//		[Export ("regularExpressionCheckingResultWithRanges:count:regularExpression:")]
//		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
//		[Internal] // FIXME
//		NSTextCheckingResult RegularExpressionCheckingResult (ref NSRange ranges, nuint count, NSRegularExpression regularExpression);

		[Static]
		[Export ("phoneNumberCheckingResultWithRange:phoneNumber:")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		NSTextCheckingResult PhoneNumberCheckingResult (NSRange range, string phoneNumber);

		[Static]
		[Export ("transitInformationCheckingResultWithRange:components:")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSTextCheckingResult TransitInformationCheckingResult (NSRange range, NSDictionary components);

		[Static]
		[Wrap ("TransitInformationCheckingResult (range, components != null ? components.Dictionary : null)")]
		NSTextCheckingResult TransitInformationCheckingResult (NSRange range, NSTextCheckingTransitComponents components);

	}

	[StrongDictionary ("NSTextChecking")]
	public interface NSTextCheckingTransitComponents {
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		string Airline { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		string Flight { get; }
	}

	[StrongDictionary ("NSTextChecking")]
	public interface NSTextCheckingAddressComponents {
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string Name { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string JobTitle { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string Organization { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string Street { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string City { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string State { get; }

		[Export ("ZipKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string ZIP { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string Country { get; }

		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		string Phone { get; }
	}

	[Static]
	public interface NSTextChecking {
		[Field ("NSTextCheckingNameKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString NameKey { get; }

		[Field ("NSTextCheckingJobTitleKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString JobTitleKey { get; }

		[Field ("NSTextCheckingOrganizationKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString OrganizationKey { get; }

		[Field ("NSTextCheckingStreetKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString StreetKey { get; }

		[Field ("NSTextCheckingCityKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString CityKey { get; }

		[Field ("NSTextCheckingStateKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString StateKey { get; }

		[Field ("NSTextCheckingZIPKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString ZipKey { get; }

		[Field ("NSTextCheckingCountryKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString CountryKey { get; }

		[Field ("NSTextCheckingPhoneKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_6)]
		NSString PhoneKey { get; }

		[Field ("NSTextCheckingAirlineKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		NSString AirlineKey { get; }

		[Field ("NSTextCheckingFlightKey")]
		[Availability (Introduced = Platform.iOS_4_0 | Platform.Mac_10_7)]
		NSString FlightKey { get; }
	}

	[BaseType (typeof(NSObject))]
	interface NSLock : NSLocking
	{
		[Export ("tryLock")]
		bool TryLock (); 

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		[Export ("name")]
		string Name { get; [NullAllowed] set; }
	}

	[BaseType (typeof(NSObject))]
	interface NSConditionLock : NSLocking {

		[DesignatedInitializer]
		[Export ("initWithCondition:")]
		IntPtr Constructor (nint condition);

		[Export ("condition")]
		nint Condition { get; }

		[Export ("lockWhenCondition:")]
		void LockWhenCondition (nint condition);

		[Export ("tryLock")]
		bool TryLock (); 

		[Export ("tryLockWhenCondition:")]
		bool TryLockWhenCondition (nint condition);

		[Export ("unlockWithCondition:")]
		void UnlockWithCondition (nint condition);

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		[Export ("lockWhenCondition:beforeDate:")]
		bool LockWhenCondition (nint condition, NSDate limit);

		[Export ("name")]
		string Name { get; [NullAllowed] set; }
	}

	[BaseType (typeof(NSObject))]
	interface NSRecursiveLock : NSLocking
	{
		[Export ("tryLock")]
		bool TryLock (); 

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		[Export ("name")]
		string Name { get; [NullAllowed] set; }
	}

	[iOS (2,0)]
	[BaseType (typeof(NSObject))]
	interface NSCondition : NSLocking
	{
		[Export ("wait")]
		void Wait ();

		[Export ("waitUntilDate:")]
		bool WaitUntilDate (NSDate limit);

		[Export ("signal")]
		void Signal ();

		[Export ("broadcast")]
		void Broadcast ();

		[Export ("name")]
		string Name { get; [NullAllowed] set; }
	}

// Not yet, the IntPtr[] argument isn't handled correctly by the generator (it tries to convert to NSArray, while the native method expects a C array).
//	[Protocol]
//	public interface NSFastEnumeration {
//		[Abstract]
//		[Export ("countByEnumeratingWithState:objects:count:")]
//		nuint Enumerate (ref NSFastEnumerationState state, IntPtr[] objects, nuint count);
//	}

	// Placeholer, just so we can start flagging things
	public interface INSFastEnumeration {}
	
#if MONOMAC
	public partial interface NSBundle {
		// - (NSImage *)imageForResource:(NSString *)name NS_AVAILABLE_MAC(10_7);
		[Lion, Export ("imageForResource:")]
		NSImage ImageForResource (string name);
	}
#endif

	public partial interface NSAttributedString {

#if MONOMAC
		[Lion, Field ("NSTextLayoutSectionOrientation", "AppKit")]
#else
		[iOS (7,0)]
		[Field ("NSTextLayoutSectionOrientation", "UIKit")]
#endif
		NSString TextLayoutSectionOrientation { get; }

#if MONOMAC
		[Lion, Field ("NSTextLayoutSectionRange", "AppKit")]
#else
		[iOS (7,0)]
		[Field ("NSTextLayoutSectionRange", "UIKit")]
#endif
		NSString TextLayoutSectionRange { get; }

#if MONOMAC
		[Lion, Field ("NSTextLayoutSectionsAttribute", "AppKit")]
#else
		[iOS (7,0)]
		[Field ("NSTextLayoutSectionsAttribute", "UIKit")]
#endif
		NSString TextLayoutSectionsAttribute { get; }

		#if !XAMCORE_2_0 && MONOMAC
		[Field ("NSCharacterShapeAttributeName", "AppKit")]
		NSString CharacterShapeAttributeName { get; }

		[Field ("NSGlyphInfoAttributeName", "AppKit")]
		NSString GlyphInfoAttributeName { get; }

		[Field ("NSSpellingStateAttributeName", "AppKit")]
		NSString SpellingStateAttributeName { get; }

		[MountainLion, Field ("NSTextAlternativesAttributeName", "AppKit")]
		NSString TextAlternativesAttributeName { get; }
		#endif

		[NoiOS, NoWatch, NoTV][Availability (Deprecated = Platform.Mac_10_11)]
		[Field ("NSUnderlineByWordMask", "AppKit")]
		nint UnderlineByWordMaskAttributeName { get; }
	}

#if MONOMAC
	partial interface NSFileManager {

		[MountainLion, Export ("trashItemAtURL:resultingItemURL:error:")]
		bool TrashItem (NSUrl url, out NSUrl resultingItemUrl, out NSError error);
	}

	partial interface NSFilePresenter {

		[MountainLion, Export ("primaryPresentedItemURL")]
		NSUrl PrimaryPresentedItemUrl { get; }
	}

	partial interface NSAttributedString {

		[Export ("boundingRectWithSize:options:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public partial interface NSHost {

		[Static, Internal, Export ("currentHost")]
		NSHost _Current { get;}

		[Static, Internal, Export ("hostWithName:")]
		NSHost _FromName (string name);

		[Static, Internal, Export ("hostWithAddress:")]
		NSHost _FromAddress (string address);

		[Export ("isEqualToHost:")]
		bool Equals (NSHost host);

		[Export ("name")]
		string Name { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }

		[Export ("names")]
		string [] Names { get; }

		[Internal, Export ("address")]
		string _Address { get; }

		[Internal, Export ("addresses")]
		string [] _Addresses  { get; }

		[Export ("hash"), Internal]
		nuint _Hash { get; }

		/* Deprecated, here for completeness:

		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		[Static, Export ("setHostCacheEnabled:")]
		void SetHostCacheEnabled (bool flag);

		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		[Static, Export ("isHostCacheEnabled")]
		bool IsHostCacheEnabled ();

		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		[Static, Export ("flushHostCache")]
		void FlushHostCache ();
		*/
	}

	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	public partial interface NSScriptCommand : NSCoding {

		[Internal]
		[Export ("initWithCommandDescription:")]
		IntPtr Constructor (NSScriptCommandDescription cmdDescription);

		[Internal]
		[Static]
		[Export ("currentCommand")]
		IntPtr GetCurrentCommand ();

		[Export ("appleEvent")]
		NSAppleEventDescriptor AppleEvent { get; }

		[Export ("executeCommand")]
		IntPtr Execute ();
		
		[Export ("evaluatedReceivers")]
		NSObject EvaluatedReceivers { get; }
	}

	[StrongDictionary ("NSScriptCommandArgumentDescriptionKeys")]
	public partial interface NSScriptCommandArgumentDescription {
		string AppleEventCode { get; set; }
		string Type { get; set;}
		string Optional { get; set; }
	}

	[StrongDictionary ("NSScriptCommandDescriptionDictionaryKeys")]
	public partial interface NSScriptCommandDescriptionDictionary {
		string CommandClass { get; set; } 
		string AppleEventCode { get; set; } 
		string AppleEventClassCode { get; set; }
		string Type { get; set;}
		string ResultAppleEventCode { get; set; }
		NSMutableDictionary Arguments { get; set; }
	}

	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	public partial interface NSScriptCommandDescription : NSCoding {

		[Internal]
		[Export ("initWithSuiteName:commandName:dictionary:")]
		IntPtr Constructor (NSString suiteName, NSString commandName, NSDictionary commandDeclaration);

		[Internal]
		[Export ("appleEventClassCode")]
		int FCCAppleEventClassCode { get; }

		[Internal]
		[Export ("appleEventCode")]
		int FCCAppleEventCode { get; }

		[Export ("commandClassName")]
		string ClassName { get; }

		[Export ("commandName")]
		string Name { get; }

		[Export ("suiteName")]
		string SuitName { get; }

		[Internal]
		[Export ("appleEventCodeForArgumentWithName:")]
		int FCCAppleEventCodeForArgument (NSString name);

		[Export ("argumentNames")]
		string [] ArgumentNames { get; }

		[Internal]
		[Export ("isOptionalArgumentWithName:")]
		bool NSIsOptionalArgument (NSString name);

		[Internal]
		[Export ("typeForArgumentWithName:")]
		NSString GetNSTypeForArgument (NSString name);

		[Internal]
		[Export ("appleEventCodeForReturnType")]
		int FCCAppleEventCodeForReturnType { get; }

		[Export ("returnType")]
		string ReturnType { get; }

		[Internal]
		[Export ("createCommandInstance")]
		IntPtr CreateCommandInstancePtr ();
	}

	[BaseType (typeof (NSObject))]
	public interface NSAffineTransform : NSSecureCoding, NSCopying {
		[Export ("initWithTransform:")]
		IntPtr Constructor (NSAffineTransform transform);

		[Export ("translateXBy:yBy:")]
		void Translate (nfloat deltaX, nfloat deltaY);

		[Export ("rotateByDegrees:")]
		void RotateByDegrees (nfloat angle);

		[Export ("rotateByRadians:")]
		void RotateByRadians (nfloat angle);

		[Export ("scaleBy:")]
		void Scale (nfloat scale);

		[Export ("scaleXBy:yBy:")]
		void Scale (nfloat scaleX, nfloat scaleY);

		[Export ("invert")]
		void Invert ();

		[Export ("appendTransform:")]
		void AppendTransform (NSAffineTransform transform);

		[Export ("prependTransform:")]
		void PrependTransform (NSAffineTransform transform);

		[Export ("transformPoint:")]
		CGPoint TransformPoint (CGPoint aPoint);

		[Export ("transformSize:")]
		CGSize TransformSize (CGSize aSize);
		
		[Export ("transformBezierPath:")]
		NSBezierPath TransformBezierPath (NSBezierPath path);

		[Export ("set")]
		void Set ();

		[Export ("concat")]
		void Concat ();

		[Export ("transformStruct")]
		CGAffineTransform TransformStruct { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface NSConnection {
		[Static, Export ("connectionWithReceivePort:sendPort:")]
		NSConnection Create ([NullAllowed] NSPort receivePort, [NullAllowed] NSPort sendPort);

		[Export ("runInNewThread")]
		void RunInNewThread ();

		// enableMultipleThreads, multipleThreadsEnabled - no-op in 10.5+ (always enabled)

		[Export ("addRunLoop:")]
		void AddRunLoop (NSRunLoop runLoop);

		[Export ("removeRunLoop:")]
		void RemoveRunLoop (NSRunLoop runLoop);

		[Static, Export ("serviceConnectionWithName:rootObject:usingNameServer:")]
		NSConnection CreateService (string name, NSObject root, NSPortNameServer server);

		[Static, Export ("serviceConnectionWithName:rootObject:")]
		NSConnection CreateService (string name, NSObject root);

		[Export ("registerName:")]
		bool RegisterName (string name);

		[Export ("registerName:withNameServer:")]
		bool RegisterName (string name, NSPortNameServer server);

		[Export ("rootObject", ArgumentSemantic.Retain)]
		NSObject RootObject { get; set; }

		[Static, Export ("connectionWithRegisteredName:host:")]
		NSConnection LookupService (string name, [NullAllowed] string hostName);

		[Static, Export ("connectionWithRegisteredName:host:usingNameServer:")]
		NSConnection LookupService (string name, [NullAllowed] string hostName, NSPortNameServer server);

		[Internal, Export ("rootProxy")]
		IntPtr _GetRootProxy ();

		[Internal, Static, Export ("rootProxyForConnectionWithRegisteredName:host:")]
		IntPtr _GetRootProxy (string name, [NullAllowed] string hostName);

		[Internal, Static, Export ("rootProxyForConnectionWithRegisteredName:host:usingNameServer:")]
		IntPtr _GetRootProxy (string name, [NullAllowed] string hostName, NSPortNameServer server);

		[Export ("remoteObjects")]
		NSObject [] RemoteObjects { get; }

		[Export ("localObjects")]
		NSObject [] LocalObjects { get; }

		[Static, Export ("currentConversation")]
		NSObject CurrentConversation { get; }

		[Static, Export ("allConnections")]
		NSConnection [] AllConnections { get; }

		[Export ("requestTimeout")]
		NSTimeInterval RequestTimeout { get; set; }

		[Export ("replyTimeout")]
		NSTimeInterval ReplyTimeout { get; set; }

		[Export ("independentConversationQueueing")]
		bool IndependentConversationQueueing { get; set; }

		[Export ("addRequestMode:")]
		void AddRequestMode (NSString runLoopMode);

		[Export ("removeRequestMode:")]
		void RemoveRequestMode (NSString runLoopMode);

		[Export ("requestModes")]
		NSString [] RequestModes { get; }

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("receivePort")]
		NSPort ReceivePort { get; }

		[Export ("sendPort")]
		NSPort SendPort { get; }

		[Export ("dispatchWithComponents:")]
		void Dispatch (NSArray components);

		[Export ("statistics")]
		NSDictionary Statistics { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSConnectionDelegate Delegate { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSConnectionDelegate {
		[Export ("authenticateComponents:withData:")]
		bool AuthenticateComponents (NSArray components, NSData authenticationData);

		[Export ("authenticationDataForComponents:")]
		NSData GetAuthenticationData (NSArray components);

		[Export ("connection:shouldMakeNewConnection:")]
		bool ShouldMakeNewConnection (NSConnection parentConnection, NSConnection newConnection);

		[Export ("connection:handleRequest:")]
		bool HandleRequest (NSConnection connection, NSDistantObjectRequest request);

		[Export ("createConversationForConnection:")]
		NSObject CreateConversation (NSConnection connection);

		[Export ("makeNewConnection:sender:")]
		bool AllowNewConnection (NSConnection newConnection, NSConnection parentConnection);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface NSDistantObjectRequest {
		[Export ("connection")]
		NSConnection Connection { get; }

		[Export ("conversation")]
		NSObject Conversation { get; }

		[Export ("invocation")]
		NSInvocation Invocation { get; }

		[Export ("replyWithException:")]
		void Reply ([NullAllowed] NSException exception);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	public interface NSPortNameServer {
		[Static, Export ("systemDefaultPortNameServer")]
		NSPortNameServer SystemDefault { get; }

		[Export ("portForName:")]
		NSPort GetPort (string portName);

		[Export ("portForName:host:")]
		NSPort GetPort (string portName, string hostName);

		[Export ("registerPort:name:")]
		bool RegisterPort (NSPort port, string portName);

		[Export ("removePortForName:")]
		bool RemovePort (string portName);
	}
	
	// FAK Left off until I understand how to do structs
	//struct NSAffineTransformStruct {
	//	public float M11, M12, M21, M22;
	//	public float tX, tY;
	//}

	[BaseType (typeof (NSCharacterSet))]
	public interface NSMutableCharacterSet {
		[Export ("removeCharactersInRange:")]
		void RemoveCharacters (NSRange aRange);

		[Export ("addCharactersInString:")]
		void AddCharacters (string aString);

		[Export ("removeCharactersInString:")]
		void RemoveCharacters (string aString);

		[Export ("formUnionWithCharacterSet:")]
		void UnionWith (NSCharacterSet otherSet);

		[Export ("formIntersectionWithCharacterSet:")]
		void IntersectWith (NSCharacterSet otherSet);

		[Export ("invert")]
		void Invert ();

	}

	[BaseType (typeof (NSObject))]
	public interface NSAppleEventDescriptor : NSSecureCoding, NSCopying {
		[Static]
		[Export ("nullDescriptor")]
		NSAppleEventDescriptor NullDescriptor { get; }

		/*		[Static]
		[Export ("descriptorWithDescriptorType:bytes:length:")]
		NSAppleEventDescriptor DescriptorWithDescriptorTypebyteslength (DescType descriptorType, void bytes, uint byteCount);

		[Static]
		[Export ("descriptorWithDescriptorType:data:")]
		NSAppleEventDescriptor DescriptorWithDescriptorTypedata (DescType descriptorType, NSData data);*/

		[Static]
		[Export ("descriptorWithBoolean:")]
		NSAppleEventDescriptor DescriptorWithBoolean (Boolean boolean);

		[Static]
		[Export ("descriptorWithEnumCode:")]
		NSAppleEventDescriptor DescriptorWithEnumCode (OSType enumerator);

		[Static]
		[Export ("descriptorWithInt32:")]
		NSAppleEventDescriptor DescriptorWithInt32 (int /* int32 */ signedInt);

		[Static]
		[Export ("descriptorWithTypeCode:")]
		NSAppleEventDescriptor DescriptorWithTypeCode (OSType typeCode);

		[Static]
		[Export ("descriptorWithString:")]
		NSAppleEventDescriptor DescriptorWithString (string str);

		/*[Static]
		[Export ("appleEventWithEventClass:eventID:targetDescriptor:returnID:transactionID:")]
		NSAppleEventDescriptor AppleEventWithEventClasseventIDtargetDescriptorreturnIDtransactionID (AEEventClass eventClass, AEEventID eventID, NSAppleEventDescriptor targetDescriptor, AEReturnID returnID, AETransactionID transactionID);*/

		[Static]
		[Export ("listDescriptor")]
		NSAppleEventDescriptor ListDescriptor { get; }

		[Static]
		[Export ("recordDescriptor")]
		NSAppleEventDescriptor RecordDescriptor { get; }

		/*[Export ("initWithAEDescNoCopy:")]
		NSObject InitWithAEDescNoCopy (const AEDesc aeDesc);

		[Export ("initWithDescriptorType:bytes:length:")]
		NSObject InitWithDescriptorTypebyteslength (DescType descriptorType, void bytes, uint byteCount);

		[Export ("initWithDescriptorType:data:")]
		NSObject InitWithDescriptorTypedata (DescType descriptorType, NSData data);

		[Export ("initWithEventClass:eventID:targetDescriptor:returnID:transactionID:")]
		NSObject InitWithEventClasseventIDtargetDescriptorreturnIDtransactionID (AEEventClass eventClass, AEEventID eventID, NSAppleEventDescriptor targetDescriptor, AEReturnID returnID, AETransactionID transactionID);*/

		[Internal]
		[Sealed]
		[Export ("initListDescriptor")]
		IntPtr _InitListDescriptor ();

		[Internal]
		[Sealed]
		[Export ("initRecordDescriptor")]
		IntPtr _InitRecordDescriptor ();

#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead")]
		[Export ("initListDescriptor")]
		NSObject InitListDescriptor ();

		[Obsolete ("Use the constructor instead")]
		[Export ("initRecordDescriptor")]
		NSObject InitRecordDescriptor ();
#endif

		/*[Export ("aeDesc")]
		const AEDesc AeDesc ();

		[Export ("descriptorType")]
		DescType DescriptorType ();*/

		[Export ("data")]
		NSData Data { get; }

		[Export ("booleanValue")]
		Boolean BooleanValue { get; }

		[Export ("enumCodeValue")]
		OSType EnumCodeValue ();

		[Export ("int32Value")]
		Int32 Int32Value { get; }

		[Export ("typeCodeValue")]
		OSType TypeCodeValue { get; }

		[Export ("stringValue")]
		string StringValue { get; }

		[Export ("eventClass")]
		AEEventClass EventClass { get; }

		[Export ("eventID")]
		AEEventID EventID { get; }

		/*[Export ("returnID")]
		AEReturnID ReturnID ();

		[Export ("transactionID")]
		AETransactionID TransactionID ();*/

		[Export ("setParamDescriptor:forKeyword:")]
		void SetParamDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Export ("paramDescriptorForKeyword:")]
		NSAppleEventDescriptor ParamDescriptorForKeyword (AEKeyword keyword);

		[Export ("removeParamDescriptorWithKeyword:")]
		void RemoveParamDescriptorWithKeyword (AEKeyword keyword);

		[Export ("setAttributeDescriptor:forKeyword:")]
		void SetAttributeDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Export ("attributeDescriptorForKeyword:")]
		NSAppleEventDescriptor AttributeDescriptorForKeyword (AEKeyword keyword);

		[Export ("numberOfItems")]
		nint NumberOfItems { get; }

		[Export ("insertDescriptor:atIndex:")]
		void InsertDescriptoratIndex (NSAppleEventDescriptor descriptor, nint index);

		[Export ("descriptorAtIndex:")]
		NSAppleEventDescriptor DescriptorAtIndex (nint index);

		[Export ("removeDescriptorAtIndex:")]
		void RemoveDescriptorAtIndex (nint index);

		[Export ("setDescriptor:forKeyword:")]
		void SetDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Export ("descriptorForKeyword:")]
		NSAppleEventDescriptor DescriptorForKeyword (AEKeyword keyword);

		[Export ("removeDescriptorWithKeyword:")]
		void RemoveDescriptorWithKeyword (AEKeyword keyword);

		[Export ("keywordForDescriptorAtIndex:")]
		AEKeyword KeywordForDescriptorAtIndex (nint index);

		/*[Export ("coerceToDescriptorType:")]
		NSAppleEventDescriptor CoerceToDescriptorType (DescType descriptorType);*/

		[Mac (10, 11)]
		[Static]
		[Export ("currentProcessDescriptor")]
		NSAppleEventDescriptor CurrentProcessDescriptor { get; }

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithDouble:")]
		NSAppleEventDescriptor FromDouble (double doubleValue);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithDate:")]
		NSAppleEventDescriptor FromDate (NSDate date);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithFileURL:")]
		NSAppleEventDescriptor FromFileURL (NSUrl fileURL);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithProcessIdentifier:")]
		NSAppleEventDescriptor FromProcessIdentifier (int processIdentifier);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithBundleIdentifier:")]
		NSAppleEventDescriptor FromBundleIdentifier (string bundleIdentifier);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithApplicationURL:")]
		NSAppleEventDescriptor FromApplicationURL (NSUrl applicationURL);

		[Mac (10, 11)]
		[Export ("doubleValue")]
		double DoubleValue { get; }

		[Mac (10,11)]
		[Export ("sendEventWithOptions:timeout:error:")]
		[return: NullAllowed]
		NSAppleEventDescriptor SendEvent (NSAppleEventSendOptions sendOptions, double timeoutInSeconds, [NullAllowed] out NSError error);

		[Mac (10, 11)]
		[Export ("isRecordDescriptor")]
		bool IsRecordDescriptor { get; }

		[Mac (10, 11)]
		[NullAllowed, Export ("dateValue", ArgumentSemantic.Copy)]
		NSDate DateValue { get; }

		[Mac (10, 11)]
		[NullAllowed, Export ("fileURLValue", ArgumentSemantic.Copy)]
		NSUrl FileURLValue { get; }
	}

	[BaseType (typeof (NSObject))]
	public interface NSAppleEventManager {
		[Static]
		[Export ("sharedAppleEventManager")]
		NSAppleEventManager SharedAppleEventManager { get; }

		[Export ("setEventHandler:andSelector:forEventClass:andEventID:")]
		void SetEventHandler (NSObject handler, Selector handleEventSelector, AEEventClass eventClass, AEEventID eventID);

		[Export ("removeEventHandlerForEventClass:andEventID:")]
#if XAMCORE_2_0
		void RemoveEventHandler (AEEventClass eventClass, AEEventID eventID);
#else
		[Obsolete ("Use RemoveEventHandler instead")]
		void RemoveEventHandlerForEventClassandEventID (AEEventClass eventClass, AEEventID eventID);
#endif

		[Export ("currentAppleEvent")]
		NSAppleEventDescriptor CurrentAppleEvent { get; }

		[Export ("currentReplyAppleEvent")]
		NSAppleEventDescriptor CurrentReplyAppleEvent { get; }

		[Export ("suspendCurrentAppleEvent")]
		NSAppleEventManagerSuspensionID SuspendCurrentAppleEvent ();

		[Export ("appleEventForSuspensionID:")]
		NSAppleEventDescriptor AppleEventForSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

		[Export ("replyAppleEventForSuspensionID:")]
		NSAppleEventDescriptor ReplyAppleEventForSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

		[Export ("setCurrentAppleEventAndReplyEventWithSuspensionID:")]
		void SetCurrentAppleEventAndReplyEventWithSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

		[Export ("resumeWithSuspensionID:")]
		void ResumeWithSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

	}

	[BaseType (typeof (NSObject))]
	public interface NSTask {
		[Export ("launch")]
		void Launch ();

		[Export ("interrupt")]
		void Interrupt ();

		[Export ("terminate")]
		void Terminate ();

		[Export ("suspend")]
		bool Suspend ();

		[Export ("resume")]
		bool Resume ();

		[Export ("waitUntilExit")]
		void WaitUntilExit ();

		[Static]
		[Export ("launchedTaskWithLaunchPath:arguments:")]
		NSTask LaunchFromPath (string path, string[] arguments);

		//Detected properties
		[Export ("launchPath")]
		string LaunchPath { get; set; }

		[Export ("arguments")]
		string [] Arguments { get; set; }

		[Export ("environment", ArgumentSemantic.Copy)]
		NSDictionary Environment { get; set; }

		[Export ("currentDirectoryPath")]
		string CurrentDirectoryPath { get; set; }

		[Export ("standardInput", ArgumentSemantic.Retain)]
		NSObject StandardInput { get; set; }

		[Export ("standardOutput", ArgumentSemantic.Retain)]
		NSObject StandardOutput { get; set; }

		[Export ("standardError", ArgumentSemantic.Retain)]
		NSObject StandardError { get; set; }

		[Export ("isRunning")]
		bool IsRunning { get; }

		[Export ("processIdentifier")]
		int ProcessIdentifier { get; } /* pid_t = int */

		[Export ("terminationStatus")]
		int TerminationStatus { get; } /* int, not NSInteger */

		[Export ("terminationReason")]
		NSTaskTerminationReason TerminationReason { get; }

		// Fields
		[Field ("NSTaskDidTerminateNotification")]
		NSString NSTaskDidTerminateNotification { get; }
	}

	[MountainLion]
	[BaseType (typeof (NSObject))]
	public interface NSUserNotification : NSCoding, NSCopying {
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }
		
		[Export ("subtitle", ArgumentSemantic.Copy)]
		string Subtitle { get; set; }
		
		[Export ("informativeText", ArgumentSemantic.Copy)]
		string InformativeText { get; set; }
		
		[Export ("actionButtonTitle", ArgumentSemantic.Copy)]
		string ActionButtonTitle { get; set; }
		
		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; set; }
		
		[Export ("deliveryDate", ArgumentSemantic.Copy)]
		NSDate DeliveryDate { get; set; }
		
		[Export ("deliveryTimeZone", ArgumentSemantic.Copy)]
		NSTimeZone DeliveryTimeZone { get; set; }
		
		[Export ("deliveryRepeatInterval", ArgumentSemantic.Copy)]
		NSDateComponents DeliveryRepeatInterval { get; set; }
		
		[Export ("actualDeliveryDate")]
		NSDate ActualDeliveryDate { get; }
		
		[Export ("presented")]
		bool Presented { [Bind("isPresented")] get; }
		
		[Export ("remote")]
		bool Remote { [Bind("isRemote")] get; }
		
		[Export ("soundName", ArgumentSemantic.Copy)]
		string SoundName { get; set; }
		
		[Export ("hasActionButton")]
		bool HasActionButton { get; set; }
		
		[Export ("activationType")]
		NSUserNotificationActivationType ActivationType { get; }
		
		[Export ("otherButtonTitle", ArgumentSemantic.Copy)]
		string OtherButtonTitle { get; set; }

		[Field ("NSUserNotificationDefaultSoundName")]
		NSString NSUserNotificationDefaultSoundName { get; }
	}
	
	[MountainLion]
	[BaseType (typeof (NSObject),
	           Delegates=new string [] {"WeakDelegate"},
	Events=new Type [] { typeof (NSUserNotificationCenterDelegate) })]
	[DisableDefaultCtor] // crash with: NSUserNotificationCenter designitated initializer is _centerForBundleIdentifier
	public interface NSUserNotificationCenter 
	{
		[Export ("defaultUserNotificationCenter")][Static]
		NSUserNotificationCenter DefaultUserNotificationCenter { get; }
		
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		NSUserNotificationCenterDelegate Delegate { get; set; }
		
		[Export ("scheduledNotifications", ArgumentSemantic.Copy)]
		NSUserNotification [] ScheduledNotifications { get; set; }
		
		[Export ("scheduleNotification:")][PostGet ("ScheduledNotifications")]
		void ScheduleNotification (NSUserNotification notification);
		
		[Export ("removeScheduledNotification:")][PostGet ("ScheduledNotifications")]
		void RemoveScheduledNotification (NSUserNotification notification);
		
		[Export ("deliveredNotifications")]
		NSUserNotification [] DeliveredNotifications { get; }
		
		[Export ("deliverNotification:")][PostGet ("DeliveredNotifications")]
		void DeliverNotification (NSUserNotification notification);
		
		[Export ("removeDeliveredNotification:")][PostGet ("DeliveredNotifications")]
		void RemoveDeliveredNotification (NSUserNotification notification);
		
		[Export ("removeAllDeliveredNotifications")][PostGet ("DeliveredNotifications")]
		void RemoveAllDeliveredNotifications ();
	}
	
	[MountainLion]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	public interface NSUserNotificationCenterDelegate 
	{
		[Export ("userNotificationCenter:didDeliverNotification:"), EventArgs ("UNCDidDeliverNotification")]
		void DidDeliverNotification (NSUserNotificationCenter center, NSUserNotification notification);
		
		[Export ("userNotificationCenter:didActivateNotification:"), EventArgs ("UNCDidActivateNotification")]
		void DidActivateNotification (NSUserNotificationCenter center, NSUserNotification notification);
		
		[Export ("userNotificationCenter:shouldPresentNotification:"), DelegateName ("UNCShouldPresentNotification"), DefaultValue (false)]
		bool ShouldPresentNotification (NSUserNotificationCenter center, NSUserNotification notification);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSAppleScript : NSCopying {

		// @required - (instancetype)initWithContentsOfURL:(NSURL *)url error:(NSDictionary **)errorInfo;
		[Export ("initWithContentsOfURL:error:")]
		IntPtr Constructor (NSUrl url, out NSDictionary errorInfo);

		// @required - (instancetype)initWithSource:(NSString *)source;
		[Export ("initWithSource:")]
		IntPtr Constructor (string source);

		// @property (readonly, copy) NSString * source;
		[Export ("source")]
		string Source { get; }

		// @property (readonly, getter = isCompiled) BOOL compiled;
		[Export ("compiled")]
		bool Compiled { [Bind ("isCompiled")] get; }

		// @required - (BOOL)compileAndReturnError:(NSDictionary **)errorInfo;
		[Export ("compileAndReturnError:")]
		bool CompileAndReturnError (out NSDictionary errorInfo);

		// @required - (NSAppleEventDescriptor *)executeAndReturnError:(NSDictionary **)errorInfo;
		[Export ("executeAndReturnError:")]
		NSAppleEventDescriptor ExecuteAndReturnError (out NSDictionary errorInfo);

		// @required - (NSAppleEventDescriptor *)executeAppleEvent:(NSAppleEventDescriptor *)event error:(NSDictionary **)errorInfo;
		[Export ("executeAppleEvent:error:")]
		NSAppleEventDescriptor ExecuteAppleEvent (NSAppleEventDescriptor eventDescriptor, out NSDictionary errorInfo);

		[Export ("richTextSource", ArgumentSemantic.Retain)]
		NSAttributedString RichTextSource { get; }
	}
#endif // MONOMAC
}
