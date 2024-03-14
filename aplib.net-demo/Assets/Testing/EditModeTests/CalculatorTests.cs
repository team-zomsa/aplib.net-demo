using NUnit.Framework;

public class NewTestScript
{
                [Test]
                [TestCase(5, 10, 15)]
                [TestCase(-2, 7, 5)]
                [TestCase(0, 0, 0)]
                public void Calculator_Addition(int a, int b, int expected)
                {
                                // Arrange
                                // Handled in testcases

                                // Act
                                int result = Calculator.Add(a, b);

                                // Assert
                                Assert.AreEqual(expected, result);
                }
}
