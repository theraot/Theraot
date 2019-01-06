using System.ComponentModel.DataAnnotations;

namespace TestRunner
{
    public class DisplayAttributeAvailabilityTest
    {
        [Display(Name = "foo", Description = "Hello World!")]
        public string MyProperty { get; set; }
    }
}