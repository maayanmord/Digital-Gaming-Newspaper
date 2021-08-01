using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DGN.Models;

namespace DGN.Data
{
    public static class DbSeed
    {
        public static void Seed(DGNContext _context)
        {
            // Categories
            if (!_context.Category.Any())
            {
                _context.Category.AddRange(
                    new Category() { CategoryName = "Shooter" },
                    new Category() { CategoryName = "MOBA" },
                    new Category() { CategoryName = "RPG" },
                    new Category() { CategoryName = "Action" },
                    new Category() { CategoryName = "Fighting" },
                    new Category() { CategoryName = "Survival" },
                    new Category() { CategoryName = "Battle Royale" },
                    new Category() { CategoryName = "Adventure" },
                    new Category() { CategoryName = "Simulation" },
                    new Category() { CategoryName = "Strategy" },
                    new Category() { CategoryName = "Sports" },
                    new Category() { CategoryName = "MMO" }
                );

                _context.SaveChanges();
            }

            // Users
            if (!_context.User.Any())
            {
                string defaultPassword = "Aa123456!";

                User clientMarionPearson = new User()
                {
                    Username = "Client",
                    Email = "Client@test.com",
                    Firstname = "Marion",
                    Lastname = "Pearson",
                    Birthday = new DateTime(1984, 6, 2),
                    ImageLocation = "/images/ClientProfile.jpg",
                    About = "Hi, My name is Marion Pearson",
                    Role = UserRole.Client
                };
                clientMarionPearson.Password = new Password(clientMarionPearson.Id, defaultPassword, clientMarionPearson);

                User authorDanParker = new User()
                {
                    Username = "Author",
                    Email = "Author@test.com",
                    Firstname = "Dan",
                    Lastname = "Parker",
                    Birthday = new DateTime(1958, 10, 3),
                    ImageLocation = "/images/AuthorProfile.jpg",
                    About = "Hi, My name is Dan Parker",
                    Role = UserRole.Author
                };
                authorDanParker.Password = new Password(authorDanParker.Id, defaultPassword, authorDanParker);

                User adminLarryRose = new User()
                {
                    Username = "Admin",
                    Email = "Admin@test.com",
                    Firstname = "Larry",
                    Lastname = "Rose",
                    Birthday = new DateTime(1995, 11, 7),
                    ImageLocation = "/images/AdminProfile.jpg",
                    About = "Hi, My name is Larry Rose",
                    Role = UserRole.Admin
                };
                adminLarryRose.Password = new Password(adminLarryRose.Id, defaultPassword, adminLarryRose);

                _context.User.AddRange(clientMarionPearson, authorDanParker, adminLarryRose);
                _context.SaveChanges();
            }

            // Articles + Comments + Likes
            if (!_context.Article.Any())
            {
                _context.Article.AddRange(
                    new Article()
                    {
                        Title = "Horizon Forbidden West is reportedly delayed to 2022",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("Adventure")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 7, 30, 17, 30, 00),
                        LastUpdatedTimestamp = new DateTime(2021, 7, 30),
                        ImageLocation = "/images/ArticleSeed1.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"PlayStation fans will have to wait until 2022 to play Horizon Forbidden West, according to Bloomberg. The outlet reports the company has delayed its next big PS5 and PS4 exclusive to next year, pushing it back from its current 2021 holiday season release timeframe.

                                Ahead of today's news, Sony hinted at a potential delay last month when the company published an interview with PlayStation Studios head Hermen Hulst. 'For Horizon, we think we are on track to release this holiday season,' Hulst said at the time. 'But that isn't quite certain yet, and we're working as hard as we can to confirm that to you as soon as we can.' At the moment, it's not clear what's behind the delay.

                                If Sony does in fact delay Forbidden West, it won't be the only first-party exclusive to miss its previously announced 2021 release date. Earlier in the year, the company delayed both Gran Turismo 7 and the next God of War entry to 2022. Just last week, Bethesda also delayed Ghostwire: Tokyo, its upcoming PlayStation 5 and PC horror game from Tango Gameworks, to early 2022.",
                        Comments = new List<Comment>()
                        {
                            new Comment() { Body = "This is so sad!", CreationTimestamp = new DateTime(2021, 7, 31, 10, 12, 00) ,User = _context.User.Where(u => u.Username.Equals("Client")).FirstOrDefault()},
                            new Comment() { Body = "I can't stop waiting for this game! :P", CreationTimestamp = new DateTime(2021, 7, 30, 20, 02, 00), User = _context.User.Where(u => u.Username.Equals("Admin")).FirstOrDefault()},
                            new Comment() { Body = "Hope you guys enjoy my article", CreationTimestamp = new DateTime(2021, 7, 30, 17, 31, 00), User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault()}
                        },
                        UserLikes = null
                    },
                    new Article()
                    {
                        Title = "Outer Wilds: Echoes of the Eye - is an expansion of a modern classic",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("Survival")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 7, 28),
                        LastUpdatedTimestamp = new DateTime(2021, 7, 29),
                        ImageLocation = "/images/ArticleSeed2.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"How do you create DLC for a game whose very purpose is to offer a confined, looping world? Honestly, after watching the trailer for Outer Wilds: Echoes of the Eye, I still have no idea. Outer Wilds was one of our favorite games of 2019, building an enthralling mystery into a non-linear exploration game that effectively restarts every 22 minutes.

                                Outer Wilds: Echoes of the Eye, the game's first and final DLC, will build on the game with new narrative threads and locales. Not much is known beyond that, but based on the strength of the original, I'm down to play an expanded version. Hopefully, the expansion will persuade more people to try out this gem of a game, which, as Devindra Hardawar wrote in our 'Favorite games of 2019'  article, 'demands patience and an adventurous spirit,' but 'promises adventure like nothing else.' Echoes of the Eye will be available September 28th for PS4, Xbox One and PC via Steam and Epic Games Store for $14.99",
                        Comments = new List<Comment>()
                        {
                            new Comment() { Body = "Love the space", CreationTimestamp = new DateTime(2021, 7, 28, 14, 42, 00) ,User = _context.User.Where(u => u.Username.Equals("Client")).FirstOrDefault()}
                        },
                        UserLikes = null
                    },
                    new Article()
                    {
                        Title = "Solar Ash brings surreal 3D platforming to PC, PS4 and PS5 on October 26th",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("Adventure")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 7, 29, 17, 22, 00),
                        LastUpdatedTimestamp = new DateTime(2021, 7, 29),
                        ImageLocation = "/images/ArticleSeed3.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"As a kid, Alx Preston spent a significant amount of time as a member of the audience, watching his brother sing in choir and opera groups. One night, he found himself sitting in a pew at the heart of a large, elegant church, letting the sounds of yet another performance wash over him. He was tired. He also happened to be playing a lot of The Legend of Zelda: Ocarina of Time at home.

                                'I kind of fell into a dream state,' Preston said. 'I was playing a lot of Ocarina of Time at the time, and so the vocals of that mixed with this kind of fantastical vision of going through a forest. I think for me that Ocarina of Time was one of those formative games that really allowed me to see what was possible within the medium.'

                                Preston was the creative energy behind Hyper Light Drifter, a pixelated 2D title that helped define a generation of neon-coated indie hits in the mid-2010s, and he’s the founder of LA-based studio Heart Machine. He and a growing team of developers have been working on their sophomore release, Solar Ash, since late 2016. It’s a third-person, 3D game set in a dreamlike sci-fi space called the Ultravoid.

                                To put it in terms of Zelda titles, Hyper Light Drifter is A Link to the Past, and Solar Ash is Ocarina of Time.

                                '[Ocarina of Time] was really the game that felt so much bigger and limitless in its scope and scale and adventures,' Preston said. 'It built a believable world that I could live in and it was 3D, and so of course I always had some idea I might jump into 3D.'

                                Today, publisher Annapurna Interactive announced Solar Ash will hit PC and PlayStation platforms on October 26th, five years after Heart Machine began working on it.

                                'Hyper Light was a way for me to get started, it was a way for me to be more grounded and put together a crew and understand, can I do this? Can I actually make games?” Preston said. “And so having answered that question, then the natural next step for me was something in 3D. Can I put something out there that really opens up the world and makes you feel like you can truly escape into something, a creation that is otherworldly, that you otherwise wouldn't have the experience of? A lot of my childhood was spent escaping into those bigger experiences.'

                                Hyper Light Drifter was a deliciously difficult 2D action RPG, and at first glance, Solar Ash looks like the third-person, 3D interpretation of that same game world. It’s bright and ethereal, with an emphasis on massive enemies and rapid-fire mechanics. The protagonist, Rei, is a slender assassin on a journey to save her home from the Ultravoid, a supermassive black hole hungry for whole planets.

                                Even though it’s 3D, Solar Ash looks so similar to Hyper Light Drifter that Preston has had to clarify whether it’s a sequel a few times over. To be clear, it’s not. But to be fair, Preston has personally contributed to the confusion.

                                'I did ambiguously say it's in the same universe,' Preston said. 'Not like Marvel Cinematic Universe, but literally in a universe. So a million galaxies away, technically sure. It's still in a universe. So it's kind of a goof way of saying it. I would say that there are connected threads between the games, because I am who I am as a creator, as an artist... but it's its own game, it's its own identity in many ways. It's not trying to say ‘I'm a sequel’ or anything like that to Drifter.'

                                Solar Ash is an action platformer with Heart Machine’s DNA baked into its code. It doesn’t attempt to do too much, and the team instead has focused on implementing a handful of core mechanics and making them feel as perfect as possible. Solar Ash is filled with radioactive environments and grotesque enemies, and it's all about fluidity and agility, surfing through the ruins of lost civilizations at the center of a black hole.

                                There are about 25 people on the Solar Ash team, including Hyper Light Drifter and It Follows composer Rich Vreeland, otherwise known as Disasterpeace. That’s a bigger dev team than the original Hyper Light Drifter crew, but then again, Solar Ash is a bigger game.

                                'For Drifter and for Solar Ash, there are similar threads of really focusing on the core elements that are impactful and getting as much mileage out of those as we possibly can,' Preston said. 'Because we have a small team making a big-ass project, and the team has been excellent in carrying through on everything that we could. Everyone's done incredible heavy lifting and worn a lot of different hats, as you have to do on this scale of team, for this scale of project.'

                                As Heart Machine’s second game, there’s a lot riding on Solar Ash. Preston has established his brand as an innovative, thoughtful developer, and Solar Ash is his chance to defend it — not only in the court of public opinion, but in his own mind.

                                'Audience expectation absolutely factors into it, but for me I'm my own worst critic,' Preston said. 'Like any artist, like any creative person, you hate your own work until you don't and then you let it go. I mostly focused on, how do we feel we're succeeding internally? Rather than, what is the audience going to expect out of that? Or, what kind of score will you get on Metacritic?'

                                Of course, if Solar Ash ends up feeling like a trippy sci-fi extrapolation of Ocarina of Time, it should be a success on all fronts.",
                        Comments = null,
                        UserLikes = new List<User>()
                        {
                            _context.User.Where(u => u.Username.Equals("Client")).FirstOrDefault()
                        }
                    },
                    new Article()
                    {
                        Title = "Apex Legends Emergence trailer shows off new playable character Seer",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("Battle Royale")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 7, 26, 15, 20, 00),
                        LastUpdatedTimestamp = new DateTime(2021, 7, 27),
                        ImageLocation = "/images/ArticleSeed4.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"As promised, Respawn Entertainment has shared a gameplay trailer showing off some of the more significant changes coming to Apex Legends as part of its upcoming Emergence season, which is slated to get underway on August 3rd. Right off the bat, we’re treated to a look at the new version of World’s Edge. Respawn has tweaked the battleground to add a molten lava fissure that runs through the center of the map.

                                The studio says it did this in part to make combat encounters that take part in and around the Sorting Facility, one of the main points of interest in World’s Edge, more dynamic, with a greater emphasis on close-quarters action. Another major element of the redesign is the addition of gondolas at two of the new points of interest. According to Respawn, the idea here was to replicate some of the “dynamic gameplay” that trains offered in the original version of the map.

                                Outside of the redesigned World’s Edge, we also get to see the game’s new Rampage LMG in action, which will allow players to knock down doors from a distance. Towards the end of the clip, Seer, Apex’s latest playable character, makes his entrance. His kit allows you to narrow in on your opponents by tracking their heartbeats while aiming down the sights of your weapon. If you find an enemy, you can send a swarm of microdrones to hunt them down. Players can avoid detection by Seer by moving slowly. During EA’s Play Live event last week, Apex Legends game director Chad Grenier said he believes the character will help vary the pace of matches, forcing players to move more methodically when Seer is on the other team.

                                Apex Legends is available to play for free on PlayStation, Xbox, PC and Nintendo Switch.",
                        Comments = null,
                        UserLikes = new List<User>() 
                        {
                            _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                            _context.User.Where(u => u.Username.Equals("Admin")).FirstOrDefault(),
                            _context.User.Where(u => u.Username.Equals("Client")).FirstOrDefault()
                        }
                    },
                    new Article()
                    {
                        Title = "Tim Burton-inspired 'Lost in Random' comes to consoles and PC on September 10th",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("Strategy")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 7, 22, 9, 11, 00),
                        LastUpdatedTimestamp = new DateTime(2021, 7, 22),
                        ImageLocation = "/images/ArticleSeed5.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"Electronic Arts will release Lost in Random, the latest entry in its ongoing Originals lineup, on September 10th, the publisher announced today during its EA Play Live event. First announced last year, the Tim Burton-inspired adventure game is the latest project from Fe developer Zoink.

                                In Lost in Random, your character Even is on a mission to save her sister, Odd. A die named Dicey will join your quest, and their abilities are essential to your success. In its moment-to-moment gameplay, Lost in Random is a mix of a third-person adventure title and deck-building games like Slay the Spire and Griftlands.

                                EA will release Lost in Random on Nintendo Switch, PC, Xbox One, PlayStation 4, PS5, Xbox One, Xbox Series X/S. On PC, it will be available on both Steam and Origin.",
                        Comments = null,
                        UserLikes = new List<User>()
                        {
                            _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                            _context.User.Where(u => u.Username.Equals("Client")).FirstOrDefault()
                        }
                    },
                    new Article()
                    {
                        Title = "Hogwarts Legacy has been delayed to 2022",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("Adventure")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 1, 13, 18, 19, 00),
                        LastUpdatedTimestamp = new DateTime(2021, 1, 13),
                        ImageLocation = "/images/ArticleSeed6.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"Budding wizards and witches will have to wait a while longer to delve into Hogwarts Legacy. The open-world Harry Potter RPG, which was supposed to arrive this year, has been delayed to 2022.

                                'We would like to thank fans from around the world on the tremendous reaction to the announcement of Hogwarts Legacy from our Portkey Games label,' publisher Warner Bros. Interactive Entertainment wrote in a statement on Twitter. 'Creating the best possible experience for all of the Wizarding World and gaming fans is paramount to us, so we are giving the game the time it needs.'

                                Hogwarts Legacy, which is set well before Harry Potter’s time in the 1800s, was reportedly the key reason behind AT&T changing its mind about selling off the Warner Bros. gaming division. When the game finally arrives, you’ll be able to play it on PC, PlayStation 4, PlayStation 5, Xbox One and Xbox Series X/S.",
                        Comments = null,
                        UserLikes = new List<User>()
                        {
                            _context.User.Where(u => u.Username.Equals("Admin")).FirstOrDefault(),
                            _context.User.Where(u => u.Username.Equals("Client")).FirstOrDefault()
                        }
                    },
                    new Article()
                    {
                        Title = "Ghost of Tsushima director's cut trailer shows off the gorgeous Iki Island",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("Action")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 6, 21, 13, 56, 00),
                        LastUpdatedTimestamp = new DateTime(2021, 6, 21),
                        ImageLocation = "/images/ArticleSeed7.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"Sony and developer Sucker Punch have revealed some more details about what's next for Jin Sakai in Ghost of Tsushima Director's Cut, an expanded edition of the 2020 action-adventure game. They also released a gorgeous new trailer for the upcoming version, which hits PlayStation 4 and PlayStation 5 on August 20th.

                                You'll be able to explore an entirely new island called Iki. Jin learns that a Mongol tribe led by a shaman named Ankhsar Khatun has taken residence there. Khatun is 'not only a conqueror of nations, but a shepherd of souls. And the danger she presents to Jin and his people is unlike any they have faced,' Sucker Punch senior writer Patrick Downs wrote on the PlayStation blog.

                                Iki is a 'lawless land of raiders and criminals' which has been out of samurai control for decades. Jin will run into pirates, smugglers and 'mad monks' on the island. He'll explore haunted caves and learn new techniques.

                                The expanded story will also delve deeper into Jin's painful past. 'With everything that has happened this past year, it’s no accident we also wanted to tell a story of healing,' Downs wrote. 'And we felt this would pose a unique and compelling challenge for Jin.'

Ghost of Tsushima Director's Cut is a $20 upgrade for owners of the original game. It costs an extra $10 to upgrade the PS5 version of either the base game or the director's cut. Ghost of Tsushima Director's Cut will also be available as a direct purchase for $60 on PS4 and $70 on PS5.",
                        Comments = new List<Comment>()
                        {
                            new Comment() { Body = "I already pre-ordered it :D", CreationTimestamp = new DateTime(2021, 6, 23, 14, 59, 00) ,User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault()},
                            new Comment() { Body = "This is lit!", CreationTimestamp = new DateTime(2021, 6, 21, 15, 18, 00), User = _context.User.Where(u => u.Username.Equals("Admin")).FirstOrDefault()},
                        },
                        UserLikes = null
                    },
                    new Article()
                    {
                        Title = "The Witcher 3: Wild Hunt is getting free DLC inspired by the Netflix series",
                        Category = _context.Category.Where(c => c.CategoryName.Equals("RPG")).FirstOrDefault(),
                        CreationTimestamp = new DateTime(2021, 7, 9, 22, 23, 00),
                        LastUpdatedTimestamp = new DateTime(2021, 7, 9),
                        ImageLocation = "/images/ArticleSeed8.jpg",
                        User = _context.User.Where(u => u.Username.Equals("Author")).FirstOrDefault(),
                        Body = @"CD Projekt Red will release its free next-gen update for the Witcher 3: Wild Hunt later this year, the studio announced today at its joint WitcherCon event with Netflix. What’s more, CDPR will release free DLC inspired by Netflix’s live-action adaptation of Andrzej Sapkowski's books. What that downloadable content will look like, the developer didn’t say, but key art the studio shared mentions 'extra items.' Take that as you will.

                                When CD Projekt Red first announced it was developing native versions of The Witcher 3: Wild Hunt for the new current generation consoles, it said the ports would include features like ray tracing and faster loading times. On Friday, it promised to share more information 'soon.' In 2020, the studio said those who already own the game on PC, PlayStation 4 and Xbox One would get the new release for free. We’ll also note here that CDPR plans to release a next-gen update for Cyberpunk 2077 sometime later this year as well.

                                Outside of an excuse to replay The Wild Hunt, fans can also look forward to watching Nightmare of the Wolf — an animated prequel film centered on Geralt’s mentor, Vesemir — next month, and, at long last, season two of The Witcher in December.",
                        Comments = new List<Comment>()
                        {
                            new Comment() { Body = "The series was so good!", CreationTimestamp = new DateTime(2021, 7, 9, 22, 50, 00) ,User = _context.User.Where(u => u.Username.Equals("Client")).FirstOrDefault()},
                            new Comment() { Body = "Loved the show, loved the game, of course I will play the DLC", CreationTimestamp = new DateTime(2021, 7, 9, 23, 00, 00), User = _context.User.Where(u => u.Username.Equals("Admin")).FirstOrDefault()},
                        },
                        UserLikes = null
                    }
                );

                _context.SaveChanges();
            }

            // Branches
            if (!_context.Branch.Any())
            {
                _context.Branch.AddRange(
                    new Branch() { 
                        BranchName = "Herzliya", 
                        Email = "Herzliya@dgn.com", 
                        LocationLatitude = 32.18626954192475, 
                        LocationLongitude = 34.85454361009017,  
                        ActivityTime = @"Tuesday 09:00-19:00
                                         Wednesday 09:00-19:00
                                         Thursday 09:00-19:00
                                         Friday 09:00-19:00
                                         Saturday 09:00-19:00
                                         Sunday 11:00-17:00",
                    },
                    new Branch()
                    {
                        BranchName = "Tel-Aviv",
                        Email = "tel-aviv@dgn.com",
                        LocationLatitude = 32.07193250985245,
                        LocationLongitude = 34.78915481563123,
                        ActivityTime = @"Tuesday 09:00-19:00
                                         Wednesday 09:00-19:00
                                         Thursday 09:00-19:00
                                         Friday 09:00-19:00
                                         Saturday 09:00-19:00
                                         Sunday 11:00-17:00",
                    }
                );

                _context.SaveChanges();
            }
        }
    }
}
