using AutoFixture;
using AutoFixture.AutoMoq;

namespace RU.Challenge.Fixtures.Customizations
{
    public class DefaultCustomization : CompositeCustomization
    {
        public DefaultCustomization()
            : base(new AutoMoqCustomization())
        { }
    }
}