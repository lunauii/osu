﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.IO.Network;
using osu.Game.Extensions;
using osu.Game.Online.API;
using osu.Game.Online.Rooms.RoomStatuses;
using osu.Game.Screens.OnlinePlay.Lounge.Components;

namespace osu.Game.Online.Rooms
{
    public class GetRoomsRequest : APIRequest<List<Room>>
    {
        private readonly RoomStatusFilter status;
        private readonly string category;

        public GetRoomsRequest(RoomStatusFilter status, string category)
        {
            this.status = status;
            this.category = category;
        }

        protected override WebRequest CreateWebRequest()
        {
            var req = base.CreateWebRequest();

            if (status != RoomStatusFilter.Open)
                req.AddParameter("mode", status.ToString().ToSnakeCase().ToLowerInvariant());

            if (!string.IsNullOrEmpty(category))
                req.AddParameter("category", category);

            return req;
        }

        protected override void PostProcess()
        {
            base.PostProcess();

            if (Response != null)
            {
                // API doesn't populate status so let's do it here.
                foreach (var room in Response)
                {
                    if (room.EndDate != null && DateTimeOffset.Now >= room.EndDate)
                        room.Status = new RoomStatusEnded();
                    else if (room.HasPassword)
                        room.Status = new RoomStatusOpenPrivate();
                    else
                        room.Status = new RoomStatusOpen();
                }
            }
        }

        protected override string Target => "rooms";
    }
}
