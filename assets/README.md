# SkipDropshipCompany

A Lethal Company mod that skip the dropship to purchase items when you're on the Company or in other limited situations.

This mod works for v73+. Maybe works for the earlier versions, but not tested.

## What it does

- Direct Ship Delivery: Purchased items are delivered directly to your ship instead of being delivered by dropship.

## When it activates

Direct ship delivery feature activates when any of these conditions are met:

- While landed on the Company
- In orbit on the first day (also the day after ejected) and routing to the Company
- In orbit on the next day after landing on the Company, still routing to the Company (in the same session only)

You can still use the dropship on the other moons as usual.

Cruiser still uses the dropship for delivery.

## Who needs to install

Host only. Clients do not need to install this mod, but it's safe to install on clients as it will have no effect.

If clients (including non-modded) purchase items, the direct ship delivery feature will also work for them.

## Where the items spawns

Purchased items will spawn inside your ship, near the default item position for the out-of-bounds items.

Each item type will be placed with a small offset to avoid overlapping.

## FAQ

### Can't use the dropship to attract enemies on eclipse day?

You can still use it.

- Purchase items in orbit after rerouting to a moon
- Purchase items after starting landing on a moon

The dropship will arrive as usual on that moon in these cases.

### Already purchased items before the conditions are met. Will they be delivered directly to the ship?

No. Only items purchased after the conditions are met will be delivered directly to the ship.

However, if you purchase additional items before the dropship arrives, they will be delivered together with the previous ones to the ship.

### Is it compatible with FasterItemDropship/LC_FasterRocket?

At least the following versions are assumed to be compatible.

- [FlipMods/FasterItemDropship](https://thunderstore.io/c/lethal-company/p/FlipMods/FasterItemDropship/) v1.3.1
- [zoomstv/FasterRocket](https://thunderstore.io/c/lethal-company/p/zoomstv/FasterRocket/) v1.0.1

### Does it work for the legacy versions of Lethal Company?

No test has been done for them. It is recommended to use Lethal Company v73 or later.

## Credit

Highly inspired by the following project.

- [Nexor/InstantBuy](https://thunderstore.io/c/lethal-company/p/Nexor/InstantBuy/)
