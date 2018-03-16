using System;
using System.Collections.Immutable;

namespace RU.Challenge.Domain.Entities
{
    public class ReleaseManager
    {
        public Guid Id { get; private set; }

        public IImmutableList<Release> Releases { get; private set; }

        public void AddRelease(Release release)
            => Releases = Releases.Add(release);

        private ReleaseManager(Guid id) : this()
            => Id = id;

        private ReleaseManager(ReleaseManager other) : this()
        {
            Id = other.Id;
            Releases = other.Releases;
        }

        private ReleaseManager()
            => Releases = ImmutableList.Create<Release>();

        public ReleaseManager Clone() => new ReleaseManager(this);

        public static ReleaseManager Create(Guid id)
            => new ReleaseManager(id);
    }
}