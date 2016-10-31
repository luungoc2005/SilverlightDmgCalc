This theme pack contains themes that can be applied to any Silverlight 4 project. It updates navigation templates that shipped with Visual Studio 2010, including the Silverlight Business Application template and the Silverlight Navigation Application template.

To use the themes:
1. Select a theme using the online preview of Silverlight 4 toolkit theme files
2. Add the corresponding folder of resource dictionaries to your Silverlight 4 project in an "Assets" subfolder
3. In "App.xaml" merge the resource dictionaries using this XAML:
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Assets/Styles.xaml"/>
                <ResourceDictionary Source="Assets/CoreStyles.xaml"/>
                <ResourceDictionary Source="Assets/SDKStyles.xaml"/>
                <ResourceDictionary Source="Assets/ToolkitStyles.xaml"/>
            </ResourceDictionary.MergedDictionaries>

4. If you are not using the Silverlight Toolkit in your project (you have not installed the toolkit and inserted a control) then you must either delete ToolkitStyles.xaml or set its build action to "None"
5. If you are not using the navigation template for your project you may delete "Styles.xaml"
6. If you are not using SDK controls in your project then you may delete SDKStyles.xaml
7. Clean and rebuild your project
8. If you find controls that are not picking up the styles then it likely means they are using keyed styles. You may fix this by removing keyed styles from your XAML pages and instead edit the theme resources to be "basedon" your keyed styles.
