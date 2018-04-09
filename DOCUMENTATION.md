
# Contents
This file provides documentation on how to use the included prefabs and scripts.
- [Energy Value](#energy-value-energyvalue)
- [Energy Device Type](#energy-device-type-energydevicetype)
- [Energy Manager](#energy-manager-energymanager)
- [Location Data](#location-data-locationdata)
- [Location Manager](#location-manager-locationmanager)
- [Location Handle](#location-handle-locationhandle)
- [Map Configuration](#map-configuration-mapconfiguration)
- [Map Manager](#map-manager-mapmanager)
- [Map Handle](#map-handle-maphandle)
- [Formula](#formula-formula)
- [Hazard Configuration](#hazard-configuration-hazardconfiguration)
- [Device Hazard Manager](#device-hazard-manager-devicehazardmanager)
- [Energy Device](#energy-device-energydevice)
- [Energy Device Slot](#energy-device-slot-energydeviceslot)
- [Energy Accumulator](#energy-accumulator-energyaccumulator)

---

# Platform

A platform that can be used to simulate spatial information and energy use and production.  
The simulation uses devices placed in the Unity scene. Each device has it's own effects to one or more simulated values.  
For example: A solar panel device will produce simulated electricity over time. The platform has a simulated time (can be matched to real time) and sun position is calculated based on a real geographic location. The position of the sun will affect the solar panel production rate.

The platform also includes a feature where the devices may affect other devices and can be randomly spawned over other devices. This can be used for example to spawn clouds over solar panels and production will decrease. Destroying or disabling the cloud will end the effect.

---

## Energy Value (EnergyValue)

### Overview
Defines a value that will be simulated. E.g. Electricity.  
Has a **value** (e.g. kWh) and a **rate** (e.g. kW). The rate is calculated and value changed over time by the [Energy Manager](#energy-manager-energymanager).

### Inspector parameters
- **Description:** Description to be displayed for this Energy Value.
- **Value Unit String:** A unit to be displayed for the value.
- **Rate Unit String:** A unit to be displayed for the rate.

### Usage
1. Create a new Energy Value -asset from Assets/Create menu: `Soludus/Energy Value`.
2. Add the value to the `Energy Targets` -list in a [Map Configuration](#map-configuration-mapconfiguration).
3. (hint)To see the current value and rate in inspector, switch inspector to [Debug mode](https://docs.unity3d.com/Manual/InspectorOptions.html).

---

## Energy Device Type (EnergyDeviceType)

### Overview
Defines a type for an [Energy Device](#energy-device-energydevice) used in energy simulation.

### Inspector parameters
- **Description:** Description to be displayed for this Energy Device Type.
- **Enabled:** Are these devices in use.
- **Model:** The prefab associated with this device type. Used by e.g. [Device Hazard Manager](#device-hazard-manager-devicehazardmanager) to spawn devices.
- **Size:** The size of the device. The visual size is **not** automatically affected by this. Effect output is usually multiplied by size. (Formulas can override this logic).
- **Require Slot:** Does this device require to be in a slot to be active.
- **Max Received Effects:** Maximum number of effects this device can receive.
- **Energy Effects:** List of effects that affect [Energy Values](#energy-value-energyvalue) over time when devices of this type are active.
    - **Output To:** The Energy Value that this effect will output to.
    - **Magnitude:** The base value of the effect.
    - **Energy Formula:** The [Formula](#formula-formula) that is used when calculating the output value of this effect. If left empty, a constant rate over time is used.
    - **Efficiency:** Multiplier for the effect output value. Should be in range 0 .. 1.
    - **Try Use Target Accumulators:** When checked, this effect will try to output to target accumulators instead of the Energy Value directly.
- **Device Effects:** List of effects that devices of this type can cause to other devices when active.
    - **Target Type:** The device type that can be affected by this effect.
    - **Efficiency Multiplier:** Output of the targeted device is multiplied by this value when this effect is active.

### Usage
1. Create a new Energy Device Type -asset from Assets/Create menu: `Soludus/Energy Device Type`.
2. Configure the effects in the inspector.
3. After creating an Energy Device, assign the Energy Device Type to the `Type`-field in the inspector.

---

## Energy Manager (EnergyManager)

### Overview
Updates the [Energy Values](#energy-value-energyvalue) based on the [Energy Devices](#energy-device-energydevice) in the scene.  
Calculations use the location and time as reported by a [Location Handle](#location-handle-locationhandle).  
Each frame, collects all devices with same output and formula and passes them to the formula to obtain the change in the energy values.

### Inspector parameters
- **Devices:** The set of devices to include.
- **Map Handle:** Handle to the current map. The same handle should be referenced everywhere.
- **Location Handle:** Handle to the current location. The same handle should be referenced everywhere.
- **Max Energy Tick Length Hours:** If the time scale is large enough that one frame delta time is longer than this value, extra time step iterations will be performed each frame.

### Usage
1. Add the Energy Manager component to an empty GameObject in the scene.
2. Populate the references in the inspector.

---

## Location Data (LocationData)

### Overview
Data describing a geographic location on earth.

### Inspector parameters
- **Latitude:** The latitude.
- **Longitude:** The longitude.
- **UTC Offset Hours:** The time zone.

### Usage
1. Create a new Location Data -asset from Assets/Create menu: `Soludus/Location`.
2. Set the values in the inspector.

---

## Location Manager (LocationManager)

### Overview
Updates a [Location Handle](#location-handle-locationhandle) based on a [Map Configuration](#map-configuration-mapconfiguration) and current timescale.

### Inspector parameters
- **Map Handle:** Handle to the current map. The same handle should be referenced everywhere.
- **Location Handle:** Handle to the current location. The same handle should be referenced everywhere.
- **Set To Local Time On Awake:** If checked, game time will be set to current local time at start instead of the `Begin Date Time` set in Map Configuration.

### Usage
1. Add the Location Manager component to an empty GameObject in the scene.
2. Populate the references in the inspector.

---

## Location Handle (LocationHandle)

### Overview
Contains the current location and time on earth and timescale.

### Usage
1. You are recommended to only use the `Location Handle` found in `Assets/Soludus/Energy/Scripts/Location`.

---

## Map Configuration (MapConfiguration)

### Overview
Configuration data for a map, also know as a game level.

### Inspector parameters
- **Location:** Geographic location.
- **Begin Date Time:** Local date and time when the map is loaded.
- **Time Scale:** Set the speed time passes. E.g. Time Scale = 2.0 means that a minute will elapse in 30 seconds.
- **Hazards:** Create a [Hazard Configuration](#hazard-configuration-hazardconfiguration) and drag it here to provide data for spawning hazardous devices.
- **Energy Targets:** The Energy Values used in this Map. You can define target values to set a target for the player.

### Usage
1. Create a new Map Configuration -asset from Assets/Create menu: `Soludus/Map Configuration`.
2. Assign it to the `Loaded Configuration` -field of a [Map Manager](#map-manager-mapmanager) in a scene.

---

## Map Manager (MapManager)

### Overview
Handles the reference to the active [Map Configuration](#map-configuration-mapconfiguration).

### Inspector parameters
- **Map Handle:** Handle to the current map. The same handle should be referenced everywhere.
- **Loaded Configuration:** The handle will point to this configuration after the scene has been loaded.

### Usage
1. Add the Map Manager component to an empty GameObject in the scene.
2. Populate the references in the inspector.

---

## Map Handle (MapHandle)

### Overview
A proxy object for referencing the currently active [Map Configuration](#map-configuration-mapconfiguration).

### Usage
1. You are recommended to only use the `Map Handle` found in `Assets/Soludus/Energy/Scripts/Map`.

---

## Formula (Formula)

### Overview
Abstract base class for formulas taking the info of the active devices and location and returning the rate at a point in time or value accumulated over time.

### Class methods

#### GetRate/3
> public abstract float GetRate(List<EnergyEffect> devices, Location location, bool ideal = false)

##### Parameters
- `List<EnergyEffect> devices` - The data of all the devices using this formula and outputting to the same energy value.
- `Location location` - The data for the location the devices are in.
- `bool ideal` - Ideal usually means that device multipliers and negative energy effects should not be counted. The `GetNormalDeviceRate()` method handles this automatically.

##### Returns
- `float` - The calculated rate.

The GetRate method calculates a rate of change based on the device and location data.

#### GetValue/6
> public abstract float GetValue(DateTimeOffset from, int iters, float delta, List<EnergyEffect> devices, Location location, bool ideal = false)

##### Parameters
- `DateTimeOffset from` - Last date and time.
- `int iters` - Number of iterations that should be performed. Simple way is to multiply `delta` by iterations if the formula does not need iterating.
- `float delta` - Delta time per one iteration in hours.
- `List<EnergyEffect> devices` - The data of all the devices using this formula and outputting to the same energy value.
- `Location location` - The data for the location the devices are in.
- `bool ideal` - Ideal usually means that device multipliers and negative energy effects should not be counted. The `GetNormalDeviceValue()` method handles this automatically.

##### Returns
- `float` - The calculated change in value.

The GetValue method calculates a change in value over a specified time period based on the device and location data.

### Usage
1. You can create new Formulas by deriving from the `Formula` class and implementing the functions (see `Assets/Soludus/Energy/Scripts/SolarEnergyFormula.cs` for an example).
2. Most formulas can use the static methods `GetNormalDeviceRate()` and `GetNormalDeviceValue()` unless they want to handle the individual devices in a custom way.

---

## Hazard Configuration (HazardConfiguration)

### Overview
Configuration data used by a [Device Hazard Manager](#device-hazard-manager-devicehazardmanager).

### Inspector parameters
- **Hazards Enabled:** Is hazard spawning enabled.
- **Max Hazard Count:** Maximum number of hazards that can be present at a time.
- **Target Cooldown:** Hazards will not target a slot until this many seconds have elapsed since the slot was previously targeted.
- **Spawn Speed Scale:** Scale the hazard spawning speed.
- **Spawn Interval Range:** The interval hazards spawn at will vary randomly between min and max.
- **Spawned Hazards:** List of hazard types to spawn.
    - **Type:** The [Energy Device Type](#energy-device-type-energydevicetype) of the hazard.
    - **Chance:** Portion of the total spawned devices will be this type. Relative to the total sum of chances.
    - **Max Count:** How many hazards of this type can be present at a time.

### Usage
1. Create a new Hazard Configuration -asset from Assets/Create menu: `Soludus/Hazard Configuration`.
2. Assign it to the `Hazards`-field of a [Map Configuration](#map-configuration-mapconfiguration).
3. Make sure a [Device Hazard Manager](#device-hazard-manager-devicehazardmanager) has been added to the scene.

---

## Device Hazard Manager (DeviceHazardManager)

### Overview
Optional manager for spawning specific devices randomly over time and assigning a random device slot as their target.  
Spawning parameters are defined in a [Hazard Configuration](#hazard-configuration-hazardconfiguration) which is assigned to the `Hazards` field of a [Map Configuration](#map-configuration-mapconfiguration).

### Inspector parameters
- **Map Handle:** Handle to the current map. The same handle should be referenced everywhere.
- **Device Slots:** The set of slots to consider as targets.

### Usage
1. Add the Device Hazard Manager component to an empty GameObject in the scene.
2. Populate the references in the inspector.

---

## Energy Device (EnergyDevice)

### Overview
Represents a device that interacts with [Energy Values](#energy-value-energyvalue) or other Energy Devices.

### Inspector parameters
- **Type:** The Energy Device Type of this device.
- **Grid:** Add a `GridData` component and assign it here to define a grid size for this device. If null, this device has a size of 1x1. Must not be null if you want to attach the device to a slot.
- **Target For Effects:** The [Energy Device Slot](#energy-device-slot-energydeviceslot) that will receive the effects from this device when it is activated. Use the `SetTarget()` method to change this at runtime.

### Class methods

#### SetTarget/1
> public void SetTarget(EnergyDeviceSlot target)

##### Parameters
- `EnergyDeviceSlot target` - The new target.

The SetTarget method changes the target of this device. The device effects are transferred to the new target.

#### AddDeviceEffect/2
> public void AddDeviceEffect(EnergyDevice device, EnergyDeviceType.DeviceEffect effect)

##### Parameters
- `EnergyDevice device` - The device that is the source of the effect.
- `EnergyDeviceType.DeviceEffect effect` - The effect data.

The AddDeviceEffect method adds the device effect to this device.

#### RemoveDeviceEffects/1
> public void RemoveDeviceEffects(EnergyDevice device)

##### Parameters
- `EnergyDevice device` - The device whose effects will be removed.

The RemoveDeviceEffects method removes all effects of the device from this device.

### Usage
1. Use GameObject/Create menu: `Soludus/Energy Device`.
2. The `Energy Device Shared Set Item` -component is used to automatically add the Energy Device to the Shared Set of devices. This is used by [Energy Manager](#energy-manager-energymanager).
3. Set the `Type` in the inspector.

---

## Energy Device Slot (EnergyDeviceSlot)

### Overview
Represents a slot for [Energy Devices](#energy-device-energydevice).

### Inspector parameters
- **Accepted Type:** Only Energy Devices of this type can attach to this slot.
- **Grid:** Add a `GridData` component and assign it here to define a grid size for this slot. Must not be null.
- **Accumulators:** _(Optional)_ [Energy Accumulators](#energy-accumulator-energyaccumulator) that will store the Energy value produced by devices in this slot.

### Class methods

#### GetDeviceInPosition/2
> public EnergyDevice GetDeviceInPosition(int x, int y)

##### Parameters
- `int x` - X coordinate of the position.
- `int y` - Y coordinate of the position.

##### Returns
- `EnergyDevice` - The device.

The GetDeviceInPosition method gets the device from the position in this slot.

#### IsValidPosition/3
> public bool IsValidPosition(EnergyDevice device, int x, int y)

##### Parameters
- `EnergyDevice device` - The device.
- `int x` - X coordinate of the position.
- `int y` - Y coordinate of the position.

##### Returns
- `bool` - True if the position is available.

The IsValidPosition method checks if the position is available for the device.

#### FindFirstAvailablePosition/3
> public bool FindFirstAvailablePosition(EnergyDevice device, out int x, out int y)

##### Parameters
- `EnergyDevice device` - The device.
- `out int x` - X coordinate of the found position.
- `out int y` - Y coordinate of the found position.

##### Returns
- `bool` - True if a free position was found.

The FindFirstAvailablePosition method finds an available position for the device in this slot.

#### TryAddToPosition/3
> public bool TryAddToPosition(EnergyDevice device, int x, int y)

##### Parameters
- `EnergyDevice device` - The device.
- `int x` - X coordinate of the position.
- `int y` - Y coordinate of the position.

##### Returns
- `bool` - True if the device was added.

The TryAddToPosition method tries to add the device to the position in this slot.

#### TryAddToFirstAvailablePosition/3
> public bool TryAddToFirstAvailablePosition(EnergyDevice device, out int x, out int y)

##### Parameters
- `EnergyDevice device` - The device.
- `out int x` - X coordinate of the position of the added device.
- `out int y` - Y coordinate of the position of the added device.

##### Returns
- `bool` - True if the device was added.

The TryAddToFirstAvailablePosition method tries to find an available position for the device in this slot and add it to that position.

### Usage
1. Use GameObject/Create menu: `Soludus/Energy Device Slot`.
2. The `Energy Device Slot Shared Set Item` -component is used to automatically add the Energy Device Slot to the Shared Set of slots. This is used by [Device Hazard Manager](#device-hazard-manager-devicehazardmanager).
3. Set the `Accepted Type` in the inspector.
4. If you want to pre-add devices to the slot, make them children of the slot object.

---

## Energy Accumulator (EnergyAccumulator)

### Overview
Object that collects "energy" over time and upon reaching capacity, releases it to the output [Energy Value](#energy-value-energyvalue).

### Inspector parameters
- **Capacity:** How much "energy" this accumumulator will collect before releasing it to the output Energy Value.
- **Output:** The Energy Value this accumulator will collect for.
- **Release Speed:** How fast the release happens.
- **Release All:** If true, the release will continue until empty, otherwise only capacity will be released.
- **Release When Zero Rate:** Release all when Energy Value rate reaches zero.
- **Adjust To Time Scale:** The capacity will be adjusted based on timescale to keep the apparent accumulation speed constant.

### Usage
1. Add the component to an Energy Device Slot and add a reference to the `Accumulators`-list.

---
