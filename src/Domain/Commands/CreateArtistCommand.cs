using MediatR;
using System;

namespace RU.Challenge.Domain.Commands
{
    public class CreateArtistCommand : IRequest
    {
        public Guid ArtistId { get; private set; }

        public int Age { get; set; }

        public string Name { get; set; }

        public CreateArtistCommand(int age, string name)
        {
            Age = age;
            Name = name;
        }

        public void SetId(Guid artistId)
        {
            ArtistId = artistId;
        }
    }
}