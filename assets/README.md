# SkipDropshipCompany

A Lethal Company mod that skips the dropship to purchase items when you're on the Company or in other limited situations.

This mod works for v73+. Maybe works for the earlier versions, but not tested.

## What it does

- Direct Ship Delivery: Purchased items are delivered directly to your ship instead of being delivered by dropship.

## When it activates

Direct ship delivery feature activates when any of these conditions are met:

- While landed on the Company
- In orbit on the first day (also the day after ejected)
- In orbit on the next day after landing on the Company, still routing to the Company (in the same session only)

You can still use the dropship on the other moons as usual.

Cruiser still uses the dropship for delivery.

## Who needs to install

Host only. Clients do not need to install this mod, but it's safe to install on clients as it will have no effect.

If clients (including non-modded) purchase items, the direct ship delivery feature will also work for them.

## Where the items spawns

Purchased items will spawn inside your ship, near the default item position for the out-of-bounds items.

Each item type will be placed with a small offset to avoid overlapping.

## Configuration

| Name | Type | Default | Description |
|:--------|:-----|:--------|:------------|
| Enabled | bool | true | Set to false to disable this mod. |
| RequireReroutingOnFirstDay | bool | false | If true, rerouting to the company will be required to skip the dropship on the first day. |

## FAQ

### Can't use the dropship to attract enemies on eclipse day?

You can still use it.

- Purchase items in orbit after rerouting to a moon
- Purchase items after starting landing on a moon

The dropship will arrive as usual on that moon in these cases.

You can change the first day behavior in configuration if your first day is eclipsed or you are using a mod like [stormytuna/EclipseOnly](https://thunderstore.io/c/lethal-company/p/stormytuna/EclipseOnly/).

### Already purchased items before the conditions are met. Will they be delivered directly to the ship?

No. Only items purchased after the conditions are met will be delivered directly to the ship.

However, if you purchase additional items before the dropship arrives, they will be delivered together with the previous ones to the ship.

This behaviour may be changed in the future version since it depends on the base game implementation.

### Is it compatible with FasterItemDropship/LC_FasterRocket?

At least the following versions are assumed to be compatible.

- [FlipMods/FasterItemDropship](https://thunderstore.io/c/lethal-company/p/FlipMods/FasterItemDropship/) v1.3.1
- [zoomstv/FasterRocket](https://thunderstore.io/c/lethal-company/p/zoomstv/FasterRocket/) v1.0.1

### Does it work for the legacy versions of Lethal Company?

No test has been done for them. It is recommended to use Lethal Company v73 or later.

## Differences from [Nexor/InstantBuy](https://thunderstore.io/c/lethal-company/p/Nexor/InstantBuy/)

`Nexor/InstantBuy` skips the dropship for all purchases by default.

This mod only skips the dropship when on the Company or in other limited situations.

`Nexor/InstantBuy` can be configured to disable specified items from instant purchases.

This mod does not have such a configuration option yet.

## Differences from [befuddled_productions/Quick_Buy_Menu](https://thunderstore.io/c/lethal-company/p/befuddled_productions/Quick_Buy_Menu/)

`befuddled_productions/Quick_Buy_Menu` adds some chat commands to purchase items quickly.

This mod uses the terminal as usual to purchase items.

`befuddled_productions/Quick_Buy_Menu` store the purchased items into the player's inventory directly.

This mod stores the purchased items on the ship directly.

## Differences from [FlipMods/FasterItemDropship](https://thunderstore.io/c/lethal-company/p/FlipMods/FasterItemDropship/)

`FlipMods/FasterItemDropship` reduces the waiting time for the dropship to arrive after purchasing an item.

This mod just spawns the purchased items directly on your ship instead of the dropship delivery.

## Differences from [zoomstv/FasterRocket](https://thunderstore.io/c/lethal-company/p/zoomstv/FasterRocket/)

`zoomstv/FasterRocket` reduces the waiting time for the dropship to arrive after landing on a moon.

This mod just spawns the purchased items directly on your ship instead of the dropship delivery.

## Credits

- Highly inspired by [Nexor/InstantBuy](https://thunderstore.io/c/lethal-company/p/Nexor/InstantBuy/).
