using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Ide
{
    /// <summary>
    /// Interaction logic for BuildWindow.xaml
    /// </summary>
    public partial class BuildWindow : Window
    {
        Storyboard Story = new Storyboard();

        public BuildWindow()
        {
            InitializeComponent();

            // "Building..." text animation
            StringAnimationUsingKeyFrames animBuilding = new StringAnimationUsingKeyFrames()
            {
                Duration = new Duration(new TimeSpan(0, 0, 4)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            animBuilding.KeyFrames.Add(new DiscreteStringKeyFrame("Building", TimeSpan.FromSeconds(0)));
            animBuilding.KeyFrames.Add(new DiscreteStringKeyFrame("Building.", TimeSpan.FromSeconds(1)));
            animBuilding.KeyFrames.Add(new DiscreteStringKeyFrame("Building..", TimeSpan.FromSeconds(2)));
            animBuilding.KeyFrames.Add(new DiscreteStringKeyFrame("Building...", TimeSpan.FromSeconds(3)));

            Storyboard.SetTargetName(animBuilding, BuildingLabel.Name);
            Storyboard.SetTargetProperty(animBuilding, new PropertyPath(Label.ContentProperty));
            Story.Children.Add(animBuilding);

            Story.Begin(this); // Pass this to prevent running animation after window is closed
        }
    }
}
