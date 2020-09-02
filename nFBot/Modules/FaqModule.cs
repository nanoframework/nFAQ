﻿using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using nFBot.Attributes;
using nFBot.Core.Models;
using nFBot.Core.Providers;


namespace nFBot.Modules
{
    public class FaqModule : BaseCommandModule
    {
        private readonly IFaqProvider _faqProvider;

        public FaqModule(IFaqProvider faqProvider)
        {
            _faqProvider = faqProvider;
        }
        
        [Command("faq")]
        [Usage("faq <tag>")]
        public async Task Faq(CommandContext ctx, string tag)
        {
            Faq faq = await _faqProvider.GetFaqByTag(tag);

            if (faq == null)
            {
                await ctx.RespondAsync("An FAQ under that tag could not be found");
                return;
            }

            DiscordUser creator = await ctx.Client.GetUserAsync(faq.Creator);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = faq.Tag,
                Description = faq.Content,
                Timestamp = faq.CreatedDate,
                Color = new Optional<DiscordColor>(new DiscordColor("#02ABF0")),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = string.IsNullOrWhiteSpace(creator.AvatarUrl) ? creator.DefaultAvatarUrl : creator.AvatarUrl,
                    Text = creator.Username + "#" + creator.Discriminator
                }
            };

            await ctx.RespondAsync(embed: embed);
        }
        
        [Command("createfaq")]
        [Usage("createfaq <tag> <content...>")]
        public async Task CreateFaq(CommandContext ctx, string tag, [RemainingText] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                await ctx.RespondAsync("You must provide some content for the FAQ");
                return;
            }

            if (await _faqProvider.GetFaqByTag(tag.ToLower()) != null)
            {
                await ctx.RespondAsync("That FAQ already exists");
                return;
            }
            
            await _faqProvider.CreateFaq(new Faq
            {
                Content = content,
                CreatedDate = DateTime.Now,
                Creator = ctx.User.Id,
                Tag = tag.ToLower()
            });

            await ctx.RespondAsync("FAQ created!");
        }
        
        [Command("deletefaq")]
        [Usage("deletefaq <tag>")]
        public async Task DeleteFaq(CommandContext ctx, string tag)
        {
            if (await _faqProvider.GetFaqByTag(tag.ToLower()) == null)
            {
                await ctx.RespondAsync("That FAQ doesn't exist");
                return;
            }
            
            await _faqProvider.DeleteFaq(tag);

            await ctx.RespondAsync("FAQ deleted!");
        }
    }
}
