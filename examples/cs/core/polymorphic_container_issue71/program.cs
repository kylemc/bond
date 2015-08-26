using System;
using FluentAssertions;
using generics;

namespace Examples
{
    using Bond;

    static class Program
    {
        static void Main()
        {
            var circleProps = new CircleProps
            {
                CircleString = "I'm a circle",
            };

            var circle1 = new Circle
            {
                Type = Type.Circle,
                Radius = 3.14,
                Props = new Bonded<CircleProps>(circleProps),
            };

            var circle2 = new Circle
            {
                Type = Type.Circle,
                Radius = 3.14,
                Props = new BondedWorkaround<CircleProps>(circleProps),
            };

            PropsShouldBeEquivalent(
                circle1.Props,
                circleProps,
                "Bonded");

            PropsShouldBeEquivalent(
                circle2.Props,
                circleProps,
                "BondedWorkaround");

            var polymorphic = new Polymorphic
            {
                Shapes =
                {
                    new BondedWorkaround<Circle>(circle2),
                    new Bonded<Circle>(circle1),
                },
            };

            PropsShouldBeEquivalent(
                polymorphic.Shapes[0].Deserialize().Props,
                circleProps,
                "Nested BondedWorkaround");

            /*
             This is where it fails
             ======================

             First Assertion
             ---------------
             FluentAssertions.Execution.AssertionFailedException was unhandled
               HResult=-2146233088
               Message=Expected subject to be Examples.CircleProps, but found Examples.ShapeProps.
 
             With configuration:
             - Use declared types and members
             - Compare enums by value
             - Match member by name (or throw)
             - Be strict about the order of items in byte arrays
 

             Second Assertion
             ----------------
             FluentAssertions.Execution.AssertionFailedException was unhandled
               HResult = -2146233088
               Message = Subject has member CircleString that the other object does not have.
 
             With configuration:
             - Use declared types and members
             - Compare enums by value
             - Match member by name(or throw)
             - Be strict about the order of items in byte arrays
            */
            PropsShouldBeEquivalent(
                polymorphic.Shapes[1].Deserialize().Props,
                circleProps,
                "Nested Bonded");
        }

        private static void PropsShouldBeEquivalent(
            IBonded<ShapeProps> actual,
            CircleProps expected,
            string scenario)
        {
            Console.WriteLine("\nStarting :: " + scenario);
            actual.Deserialize().GetType().ShouldBeEquivalentTo(expected.GetType());
            expected.ShouldBeEquivalentTo(actual.Deserialize()); // FA assertion order is reversed to detect missing members
            Console.WriteLine("Complete :: " + scenario);
        }
    }
}
