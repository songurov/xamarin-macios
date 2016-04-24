//
// UICollectionViewLayout.cs: extensions for the binding
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2012, Xamarin Inc.
//

#if !WATCH

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.UIKit {

	public partial class UICollectionViewLayout {

		public void RegisterClassForDecorationView (Type viewType, NSString kind)
		{
			RegisterClassForDecorationView (viewType == null ? IntPtr.Zero : Class.GetHandle (viewType), kind);
		}

		public UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView (UICollectionElementKindSection section, NSIndexPath indexPath)
		{
			NSString kind;
			switch (section) {
			case UICollectionElementKindSection.Header:
				kind = UICollectionElementKindSectionKey.Header;
				break;
			case UICollectionElementKindSection.Footer:
				kind = UICollectionElementKindSectionKey.Footer;
				break;
			default:
				throw new ArgumentOutOfRangeException ("section");
			}

			return LayoutAttributesForSupplementaryView (kind, indexPath);			
		}
	}
}

#endif // !WATCH