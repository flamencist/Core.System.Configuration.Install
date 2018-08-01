using System.ComponentModel;
using System.Configuration.Install;

namespace System.Diagnostics
{
	/// <summary>Specifies an installer for the <see cref="T:System.Diagnostics.PerformanceCounter" /> component.</summary>
	public class PerformanceCounterInstaller : ComponentInstaller
	{
		private string _categoryName = string.Empty;

		private readonly CounterCreationDataCollection _counters = new CounterCreationDataCollection();

		private string _categoryHelp = string.Empty;

		private UninstallAction _uninstallAction;

		private PerformanceCounterCategoryType _categoryType = PerformanceCounterCategoryType.Unknown;

		/// <summary>Gets or sets the performance category name for the performance counter.</summary>
		/// <returns>The performance category name for the performance counter.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value is set to null. </exception>
		/// <exception cref="T:System.ArgumentException">The value is not a valid category name.</exception>
		[DefaultValue("")]
		[ResDescription("PCCategoryName")]
		public string CategoryName
		{
			get => _categoryName;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				CheckValidCategory(value);
				_categoryName = value;
			}
		}

		/// <summary>Gets or sets the descriptive message for the performance counter.</summary>
		/// <returns>The descriptive message for the performance counter.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value is set to null. </exception>
		[DefaultValue("")]
		[ResDescription("PCI_CategoryHelp")]
		public string CategoryHelp
		{
			get => _categoryHelp;
			set => _categoryHelp = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>Gets or sets the performance counter category type.</summary>
		/// <returns>One of the <see cref="T:System.Diagnostics.PerformanceCounterCategoryType" /> values. </returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value is not a <see cref="T:System.Diagnostics.PerformanceCounterCategoryType" />.</exception>
		[DefaultValue(PerformanceCounterCategoryType.Unknown)]
		[ResDescription("PCI_IsMultiInstance")]
		public PerformanceCounterCategoryType CategoryType
		{
			get => _categoryType;
			set
			{
				if (value < PerformanceCounterCategoryType.Unknown || value > PerformanceCounterCategoryType.MultiInstance)
				{
					throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PerformanceCounterCategoryType));
				}
				_categoryType = value;
			}
		}

		/// <summary>Gets a collection of data that pertains to the counters to install.</summary>
		/// <returns>A <see cref="T:System.Diagnostics.CounterCreationDataCollection" /> that contains the names, help messages, and types of the counters to install.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ResDescription("PCI_Counters")]
		public CounterCreationDataCollection Counters => _counters;

		/// <summary>Gets a value that indicates whether the performance counter should be removed at uninstall time.</summary>
		/// <returns>One of the <see cref="T:System.Configuration.Install.UninstallAction" /> values. The default is Remove.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value is not an <see cref="T:System.Configuration.Install.UninstallAction" />.</exception>
		[DefaultValue(UninstallAction.Remove)]
		[ResDescription("PCI_UninstallAction")]
		public UninstallAction UninstallAction
		{
			get => _uninstallAction;
			set
			{
				if (!Enum.IsDefined(typeof(UninstallAction), value))
				{
					throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(UninstallAction));
				}
				_uninstallAction = value;
			}
		}

		/// <summary>Copies all the properties from the specified component that are required at install time for a performance counter.</summary>
		/// <param name="component">The component to copy from. </param>
		/// <exception cref="T:System.ArgumentException">The specified component is not a <see cref="T:System.Diagnostics.PerformanceCounter" />.-or- The specified <see cref="T:System.Diagnostics.PerformanceCounter" /> is incomplete.-or- Multiple counters in different categories are trying to be installed. </exception>
		public override void CopyFromComponent(IComponent component)
		{
			
		}

		private static void CheckValidCategory(string categoryName)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException(nameof(categoryName));
			}
			if (!CheckValidId(categoryName))
			{
				throw new ArgumentException(Res.GetString("PerfInvalidCategoryName", 1, 253));
			}
		}

		private static bool CheckValidId(string id)
		{
			if (id.Length == 0 || id.Length > 253)
			{
				return false;
			}
			for (var i = 0; i < id.Length; i++)
			{
				var c = id[i];
				if ((i == 0 || i == id.Length - 1) && c == ' ')
				{
					return false;
				}
				if (c == '"')
				{
					return false;
				}
				if (char.IsControl(c))
				{
					return false;
				}
			}
			return true;
		}
	}
}
