using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yuki.Mvvm.Attributes;

namespace Yuki.Mvvm.ViewModels
{
    /// <summary>
    ///     The class responsible for managing property dependencies on <see cref="ViewModel"/>s.
    /// </summary>
    internal static class PropertyDependencyManager
    {
        private static List<ViewModelDependencySet> ViewModelDependencies { get; }

        static PropertyDependencyManager()
        {
            ViewModelDependencies = new List<ViewModelDependencySet>();
        }

        /// <summary>
        ///     Gets the <see cref="ViewModelDependencySet"/> for the given <see cref="ViewModel"/>.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ViewModel"/> to get the <see cref="ViewModelDependencySet"/> for.
        /// </param>
        /// <returns>
        ///     The <see cref="ViewModelDependencySet"/>.
        /// </returns>
        public static ViewModelDependencySet For(ViewModel viewModel)
        {
            Type viewModelType = viewModel.GetType();
            ViewModelDependencySet dependencySet = ViewModelDependencies.First(d => d.ViewModelType == viewModelType);
            return dependencySet;
        }

        /// <summary>
        ///     Invokes the <see cref="InitializeDependenciesFor"/> if it has not already been done.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ViewModel"/> to initialize the <see cref="ViewModelDependencySet"/> for.
        /// </param>
        public static void EnsureDependenciesInitializedFor(ViewModel viewModel)
        {
            Type viewModelType = viewModel.GetType();
            if (ViewModelDependencies.All(d => d.ViewModelType != viewModelType))
            {
                InitializeDependenciesFor(viewModel);
            }
        }
        
        /// <summary>
        ///     Initializes a new <see cref="ViewModelDependencySet"/> for the given <see cref="ViewModel"/>, and stores
        ///     it internally for future use.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ViewModel"/> to initialize the <see cref="ViewModelDependencySet"/> for.
        /// </param>
        public static void InitializeDependenciesFor(ViewModel viewModel)
        {
            Type viewModelType = viewModel.GetType();

            // Make sure the ViewModel doesn't already have a dependency set created
            if (ViewModelDependencies.Any(d => d.ViewModelType == viewModelType))
            {
                throw new Exception($"A {ViewModelDependencies} already exists for the {viewModelType.Name}.");
            }

            PropertyInfo[] properties = viewModelType.GetProperties();
            List<string> getOnlyProperties = new List<string>();
            List<PropertyDependencyInfo> dependencyInfo = new List<PropertyDependencyInfo>();
            foreach (PropertyInfo property in properties)
            {
                bool hadAttribute = false;
                
                // Check for ParentAttributes
                ParentAttribute parentAttribute = property.GetCustomAttribute<ParentAttribute>();
                if (parentAttribute != null && 
                    parentAttribute.DependantPropertyNames.Any())
                {
                    hadAttribute = true;
                    
                    // If this property is already a dependency on another, then just add to that list,
                    //     Otherwise create a new entry
                    PropertyDependencyInfo existingInfo =
                        dependencyInfo.FirstOrDefault(d => d.ParentPropertyName == property.Name);
                    if (existingInfo != null)
                    {
                        existingInfo.DependantPropertyNames.AddRange(parentAttribute.DependantPropertyNames);
                    }
                    else
                    {
                        dependencyInfo.Add(new PropertyDependencyInfo(property.Name, parentAttribute.DependantPropertyNames));
                    }
                }

                // Check for DependantAttributes
                DependantAttribute dependantAttribute = property.GetCustomAttribute<DependantAttribute>();
                if (dependantAttribute != null && 
                    dependantAttribute.ParentPropertyNames.Any())
                {
                    hadAttribute = true;
                    foreach (string parentPropertyName in dependantAttribute.ParentPropertyNames)
                    {
                        // Check if the parent already has an entry and add to it's dependency list,
                        //     Otherwise create a new entry
                        PropertyDependencyInfo existingInfo =
                            dependencyInfo.FirstOrDefault(d => d.ParentPropertyName == parentPropertyName);
                        if (existingInfo != null)
                        {
                            existingInfo.DependantPropertyNames.Add(property.Name);
                        }
                        else
                        {
                            dependencyInfo.Add(new PropertyDependencyInfo(parentPropertyName, property.Name));
                        }
                    }   
                }
                
                // Check if it's a get-only property, so long as it hasn't already been accounted for by any attributes
                if (!hadAttribute && 
                    (property.GetMethod != null && 
                    property.SetMethod == null))
                {
                    getOnlyProperties.Add(property.Name);
                }
            }
            
            ViewModelDependencySet set = new ViewModelDependencySet(viewModelType, getOnlyProperties, dependencyInfo);
            ViewModelDependencies.Add(set);
        }
    }
}