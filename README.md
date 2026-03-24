# Jelly Hole Prototype (Dima_D2)

## Overview

This project is a mobile-first (Android) 3D top-down prototype inspired by games like Hole.io / Jelly Hole.

The player controls a hole that can absorb objects smaller than its current size. By absorbing objects, the hole progresses through discrete size levels and gains the ability to consume larger objects.

Each level has a timer and specific objectives (collecting certain item types). The goal is to complete objectives before time runs out.

This project focuses on core gameplay, not full production features.

---

## Target Platform

* Android (mobile-first)
* Touch controls
* Portrait orientation (preferred)

---

## Core Gameplay Loop

1. Start level
2. Move the hole using touch input
3. Absorb valid objects (smaller than hole size)
4. Gain progress toward next size level
5. Level up hole size (discrete steps, not continuous scaling)
6. Consume larger objects
7. Complete objectives before timer ends
8. Win or fail

---

## Core Systems

### 1. Player / Hole Controller

* Top-down movement on a flat surface
* Touch input (drag or joystick-like behavior)
* Smooth and responsive movement

### 2. Hole Size System

* Hole has a `SizeLevel`
* Each level has:

  * Defined radius
  * Progress threshold
* Progress increases when objects are absorbed
* Next level requires **2x more progress than previous**
* Size changes only on level-up (discrete growth)

### 3. Absorption System

* Each object has a size value
* Object can be absorbed if:

  * objectSize <= holeSize
* On absorb:

  * object is removed
  * progress is increased
  * optional feedback (scale, animation, etc.)

### 4. Collectible Items

* Each object has:

  * size
  * type (for objectives)
* Examples: fruit, desserts, etc.

### 5. Objective System

* Each level defines:

  * list of required item types
  * required counts
* Absorbing matching items updates progress
* Level is completed when all objectives are met

### 6. Timer System

* Each level has a countdown timer
* If time reaches 0 → fail
* Timer value can vary per level

### 7. Level Flow System

* Start → Gameplay → Win / Fail
* Basic restart capability

---

## Boosters

### Pre-Level Boosters

* Time Boost (increase level duration)
* Size Boost (start with higher size level)

### In-Level Boosters (MVP subset)

* Freeze Time (pause timer for short duration)
* Size Boost (temporary or instant level increase)

### Optional (Phase 2)

* Vacuum Funnel (increase absorption radius)
* Locator (highlight required items)

---

## UI (Basic)

* Timer display
* Current size level
* Objective panel (item icons + counts)
* Win / Fail screen
* Pre-level booster selection (simple)

UI should be minimal and functional, not polished.

---

## Technical Guidelines

* Prefer simple and performant solutions
* Avoid heavy physics simulation
* Use trigger-based detection for absorption
* Keep update loops minimal
* Use simple meshes and materials
* Design with mobile performance in mind

---

## TopDown Engine Usage

If TopDown Engine is used:

* Use it as a base framework only
* Implement hole gameplay systems as custom logic
* Do NOT modify engine core files unless absolutely necessary

---

## Scope (Important)

### Included

* Core movement
* Absorption logic
* Size progression (discrete levels)
* Timer
* Objectives
* Basic UI
* Basic boosters

### Not Included (for now)

* Multiplayer
* Ads / monetization implementation
* In-app purchases
* Meta progression (economy, base building, etc.)
* Live events
* Advanced UI polish
* Deep optimization passes
* Save/load system beyond minimal needs

---

## Development Phases

### Phase 1 (Core MVP)

* Movement
* Absorption
* Size levels
* Timer
* Objectives
* Win/Fail

### Phase 2 (Gameplay Depth)

* Pre-level boosters
* Freeze time
* Size boost

### Phase 3 (Extensions)

* Funnel / locator
* Juice / feedback
* Mobile tuning

---

## Goal of This Prototype

Build a clean, playable vertical slice of a Jelly Hole-like game that demonstrates:

* satisfying core loop
* readable progression
* basic mobile UX

Focus on clarity and functionality over polish.
