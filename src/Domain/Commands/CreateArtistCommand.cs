﻿using MediatR;

namespace RU.Challenge.Domain.Commands
{
    public class CreateArtistCommand : IRequest
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public CreateArtistCommand(int age, string name)
        {
            Age = age;
            Name = name;
        }
    }
}