using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateArtistCommand : IRequest
    {
        public Guid ArtistId { get; set; }

        public int Age { get; set; }

        public string Name { get; set; }

        public CreateArtistCommand(int age, string name)
        {
            Age = age;
            Name = name;
        }

        public CreateArtistCommand(Guid artistId, int age, string name)
            :this(age, name)
        {
            ArtistId = artistId;
        }
    }
}