# Tools

Measurement tools for running programmable block (PB) scripts on a worker thread (SE-626).
They are not part of the plugin build. Linux only so far.

## PbScan

Compiles PB scripts the way the game does (same wrapper class, implicit usings, references and
C# 6) and lists the game API members each one uses, from the Roslyn semantic model.

```bash
dotnet build -c Release Tools/PbScan
dotnet Tools/PbScan/bin/Release/net10.0/PbScan.dll <Bin64> <corpus> scan.json
python3 Tools/PbScan/aggregate.py scan.json members.tsv [details.json]
```

`<corpus>` holds one folder per script with its `.cs` files, for example Workshop downloads.
`details.json` is the Steam `GetPublishedFileDetails` answer for the same ids, used for names.
Scripts that no longer compile (removed API) are listed and left out of the counts.

## PbProfile

Times every PB run on an isolated Magnetar server.

- `build_world.py` puts Workshop blueprints whose PBs carry a program into a copy of a base world,
  1.5 km apart, with fresh entity ids, one owner, physics on and full batteries.
- `probe-plugin` is a source-only plugin. It times each `ExecuteCode` call (the whole run) and
  `RunSandboxedProgramActionCore` (the script alone), and writes per-run, per-frame and per-PB
  tables once the measured window ends.
- `rig.py` starts the server in a claimed test slot (`testing/parallel-test-work` in the
  workspace), optionally with this repo's Performance plugin and config options, and optionally
  takes a `dotnet-trace` sample profile in the measured window.
- `analyze.py` turns one run's tables into a Markdown summary: per-frame and per-run
  distributions, the share spent before the script starts, an estimate for N workers, the PBs
  that cost most and, with a trace, script code against game API time.

```bash
python3 Tools/PbProfile/build_world.py <base world zip> <blueprint folders...> --out worlds/PbProfile
PB_SLOT=<slot> python3 Tools/PbProfile/rig.py worlds/PbProfile --bench [--performance --option FixTerminal=true] [--trace 60]
python3 Tools/PbProfile/analyze.py ~/.se-test/<task>-ds0/probe --corpus <corpus> --titles details.json
```

The sampled split is approximate. .NET stops a thread for a sample at a GC poll, so time in leaf
frames without one is charged to the frame they return into. The stopwatch numbers from the
probe are exact.

## ConnectedShips

Ships joined by locked connectors (SE-798) on an isolated Magnetar server. `rig.py` holds the
server and client setup, in a claimed test slot (`testing/parallel-test-work` in the workspace),
with paths overridable by the `CS_*` variables it lists. `probe` is a source-only plugin driven
through files: it spawns and docks ships, gives input and reports what each ship is doing.

- `test_connected.py`: the rule, settings kept, undocking, a static grid, gravity on a moon, crash,
  clean restart, the option. Prints PASS or FAIL per check.
- `mp_check.py`: a real headless client over DirectTransport flying the ships, run under the
  se-remote project (`uv run --project <remote>/skills/se-remote python mp_check.py`). The client
  also needs Bugfixes (`CS_BUGFIXES`).
- `fight.py <option 0|1> [--no-bench-lock]`: how fast a group gets and how much force is lost.
- `settle_trace.py <option 0|1> <seconds> [--no-bench-lock]`: speed of a stopping group every 5 s.
- `docked_bench.py`: physics cost of ships docked to a static grid, fixed against forced dynamic
  (SE-797), without Performance. Alone on the machine.
