import json
import os
import subprocess
import threading
from pathlib import Path
from typing import Tuple, Dict, Optional

import click
from openai import OpenAI

# -----------------------------
# Config loading
# -----------------------------


class ToolConfig:
    def __init__(
        self,
        blender_path: str,
        output_dir: str,
        default_model_name: str,
        pipeline: str,
        materials: Dict[str, str],
    ):
        self.blender_path = blender_path
        self.output_dir = output_dir
        self.default_model_name = default_model_name
        self.pipeline = pipeline
        self.materials = materials

    @staticmethod
    def load(config_path: Optional[Path] = None) -> "ToolConfig":
        if config_path is None:
            config_path = Path(__file__).with_name("unity_asset_config.json")

        if config_path.exists():
            data = json.loads(config_path.read_text(encoding="utf-8"))
        else:
            # Defaults if config is missing
            data = {
                "blender_path": "C:/Program Files/Blender Foundation/Blender/blender.exe",
                "output_dir": "unity_assets",
                "default_model_name": "model",
                "pipeline": "HDRP",
                "materials": {
                    "wood": "HDRP/Lit",
                    "brass": "HDRP/Lit",
                    "red_inset": "HDRP/Lit",
                },
            }

        return ToolConfig(
            blender_path=data.get(
                "blender_path",
                "C:/Program Files/Blender Foundation/Blender/blender.exe",
            ),
            output_dir=data.get("output_dir", "unity_assets"),
            default_model_name=data.get("default_model_name", "model"),
            pipeline=data.get("pipeline", "HDRP"),
            materials=data.get(
                "materials",
                {
                    "wood": "HDRP/Lit",
                    "brass": "HDRP/Lit",
                    "red_inset": "HDRP/Lit",
                },
            ),
        )


# -----------------------------
# ChatGPT 3D + MTL generator
# -----------------------------


class ChatGPT3DGenerator:
    def __init__(self, model: str = "gpt-4.1"):
        api_key = os.getenv("OPENAI_API_KEY")
        if not api_key:
            raise RuntimeError("OPENAI_API_KEY environment variable is not set.")
        self.client = OpenAI(api_key=api_key)
        self.model = model

    def generate_obj_mtl(self, prompt: str, mtl_name: str) -> Tuple[str, str]:
        system_prompt = (
            "You are a 3D mesh generator. Output ONLY raw text, no markdown.\n"
            "You must output two sections:\n"
            "===OBJ===\n"
            "<Wavefront OBJ text>\n"
            "===MTL===\n"
            "<Wavefront MTL text>\n\n"
            "Requirements:\n"
            f"- The OBJ must reference the MTL using: mtllib {mtl_name}\n"
            "- Use materials named: wood, brass, red_inset\n"
            "- OBJ must include vertices (v) and faces (f) at minimum.\n"
        )

        user_prompt = (
            f"Generate OBJ + MTL for this object. Use mtllib {mtl_name}.\n\n{prompt}"
        )

        completion = self.client.chat.completions.create(
            model=self.model,
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt},
            ],
            temperature=0.2,
        )

        raw = completion.choices[0].message.content.strip()
        obj_text, mtl_text = self._split_obj_mtl(raw)
        return obj_text.strip(), mtl_text.strip()

    @staticmethod
    def _split_obj_mtl(raw: str) -> Tuple[str, str]:
        obj_marker = "===OBJ==="
        mtl_marker = "===MTL==="

        if obj_marker not in raw or mtl_marker not in raw:
            raise ValueError("Response missing OBJ/MTL markers.")

        _, rest = raw.split(obj_marker, 1)
        obj_text, mtl_text = rest.split(mtl_marker, 1)
        return obj_text, mtl_text


# -----------------------------
# ChatGPT material color advisor
# -----------------------------


class ChatGPTMaterialColorAdvisor:
    def __init__(self, model: str = "gpt-4.1"):
        api_key = os.getenv("OPENAI_API_KEY")
        if not api_key:
            raise RuntimeError("OPENAI_API_KEY environment variable is not set.")
        self.client = OpenAI(api_key=api_key)
        self.model = model

    def get_colors(self) -> Dict[str, tuple]:
        system_prompt = (
            "Output ONLY JSON with RGBA values (0-1 floats) for materials:\n"
            "wood, brass, red_inset\n"
            "Format:\n"
            "{\n"
            '  "wood": [r,g,b,a],\n'
            '  "brass": [r,g,b,a],\n'
            '  "red_inset": [r,g,b,a]\n'
            "}\n"
        )

        completion = self.client.chat.completions.create(
            model=self.model,
            messages=[{"role": "system", "content": system_prompt}],
            temperature=0.2,
        )

        import json as _json

        data = _json.loads(completion.choices[0].message.content.strip())
        return {k: tuple(float(x) for x in v) for k, v in data.items()}


# -----------------------------
# Blender integration (Windows)
# -----------------------------


def run_blender_apply_colors_and_export_fbx(
    blender_path: str,
    obj_path: Path,
    colors: Dict[str, tuple],
    fbx_output_path: Path,
) -> None:
    """
    Runs Blender in background mode to:
    - Import OBJ
    - Apply material colors
    - Export FBX
    """
    script = f"""
import bpy

colors = {colors}

# Delete default objects
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete()

# Import OBJ
bpy.ops.import_scene.obj(filepath=r"{obj_path}")

# Apply colors to materials
for mat_name, rgba in colors.items():
    mat = bpy.data.materials.get(mat_name)
    if mat is None:
        mat = bpy.data.materials.new(mat_name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = rgba

# Export FBX
bpy.ops.export_scene.fbx(
    filepath=r"{fbx_output_path}",
    embed_textures=False,
    apply_unit_scale=True,
    bake_space_transform=True
)
"""

    temp_script = obj_path.parent / "_apply_colors_and_export.py"
    temp_script.write_text(script, encoding="utf-8")

    subprocess.run(
        [
            blender_path,
            "--background",
            "--python",
            str(temp_script),
        ],
        check=True,
    )

    temp_script.unlink(missing_ok=True)


# -----------------------------
# Core generation helpers
# -----------------------------


def create_unity_style_structure(base_output_dir: Path, asset_name: str) -> Dict[str, Path]:
    """
    Creates:
        unity_assets/
            asset_name/
                Models/
                Materials/
                Prefabs/
    Returns dict with paths for Models, Materials, Prefabs.
    """
    asset_root = base_output_dir / asset_name
    models_dir = asset_root / "Models"
    materials_dir = asset_root / "Materials"
    prefabs_dir = asset_root / "Prefabs"

    models_dir.mkdir(parents=True, exist_ok=True)
    materials_dir.mkdir(parents=True, exist_ok=True)
    prefabs_dir.mkdir(parents=True, exist_ok=True)

    return {
        "root": asset_root,
        "models": models_dir,
        "materials": materials_dir,
        "prefabs": prefabs_dir,
    }


def generate_single_asset(
    cfg: ToolConfig,
    prompt: str,
    asset_name: str,
    use_blender: bool,
    model: str = "gpt-4.1",
) -> None:
    """
    Generates OBJ+MTL (and optionally FBX) for a single prompt.
    Uses Unity-style folder structure (Option C).
    """
    base_output_dir = Path(cfg.output_dir)
    base_output_dir.mkdir(parents=True, exist_ok=True)

    dirs = create_unity_style_structure(base_output_dir, asset_name)

    gen = ChatGPT3DGenerator(model=model)
    mtl_filename = f"{asset_name}.mtl"

    obj_text, mtl_text = gen.generate_obj_mtl(prompt, mtl_filename)

    obj_path = dirs["models"] / f"{asset_name}.obj"
    mtl_path = dirs["models"] / f"{asset_name}.mtl"

    obj_path.write_text(obj_text, encoding="utf-8")
    mtl_path.write_text(mtl_text, encoding="utf-8")

    if use_blender:
        advisor = ChatGPTMaterialColorAdvisor(model=model)
        colors = advisor.get_colors()
        fbx_path = dirs["models"] / f"{asset_name}.fbx"
        run_blender_apply_colors_and_export_fbx(
            cfg.blender_path,
            obj_path,
            colors,
            fbx_path,
        )


# -----------------------------
# CLI (click)
# -----------------------------


@click.group()
def cli():
    """Unity Asset Tool - Generate Unity-ready assets from text prompts."""
    pass


@cli.command("generate")
@click.argument("prompt")
@click.option("--name", default=None, help="Asset name (folder + file base name)")
@click.option("--blender", is_flag=True, help="Use Blender to color + export FBX")
@click.option("--model", default="gpt-4.1", help="ChatGPT model name")
def cli_generate(prompt: str, name: Optional[str], blender: bool, model: str):
    """
    Generate a single Unity-style asset from a prompt.
    """
    cfg = ToolConfig.load()
    asset_name = name or cfg.default_model_name

    click.echo(f"[INFO] Generating asset '{asset_name}' from prompt:")
    click.echo(f"       {prompt!r}")
    generate_single_asset(cfg, prompt, asset_name, blender, model=model)
    click.echo("[INFO] Done.")


@cli.command("generate-batch")
@click.argument("prompts_file", type=click.Path(exists=True))
@click.option("--blender", is_flag=True, help="Use Blender to color + export FBX for each")
@click.option("--model", default="gpt-4.1", help="ChatGPT model name")
def cli_generate_batch(prompts_file: str, blender: bool, model: str):
    """
    Generate multiple assets from a text file (one prompt per line).
    """
    cfg = ToolConfig.load()
    base_output_dir = Path(cfg.output_dir)
    base_output_dir.mkdir(parents=True, exist_ok=True)

    with open(prompts_file, "r", encoding="utf-8") as f:
        lines = [ln.strip() for ln in f.readlines() if ln.strip()]

    if not lines:
        click.echo("[WARN] No prompts found in file.")
        return

    click.echo(f"[INFO] Generating {len(lines)} assets from {prompts_file!r}")

    for idx, prompt in enumerate(lines):
        asset_name = f"{cfg.default_model_name}_{idx:03d}"
        click.echo(f"[BATCH] {idx+1}/{len(lines)} -> {asset_name}: {prompt!r}")
        generate_single_asset(cfg, prompt, asset_name, blender, model=model)

    click.echo("[INFO] Batch generation complete.")


# -----------------------------
# GUI (Tkinter)
# -----------------------------


def _run_gui():
    import tkinter as tk
    from tkinter import scrolledtext, messagebox

    cfg = ToolConfig.load()

    class UnityAssetGUI:
        def __init__(self, root):
            self.root = root
            self.root.title("Unity Asset Generator")

            self.prompt_label = tk.Label(root, text="Prompt:")
            self.prompt_label.pack(anchor="w")

            self.prompt_text = scrolledtext.ScrolledText(root, height=6, width=70)
            self.prompt_text.pack(fill="both", expand=True)

            self.name_label = tk.Label(root, text="Asset Name (optional):")
            self.name_label.pack(anchor="w")

            self.name_entry = tk.Entry(root)
            self.name_entry.pack(fill="x")

            self.blender_var = tk.BooleanVar(value=False)
            self.blender_check = tk.Checkbutton(
                root,
                text="Use Blender (color + FBX)",
                variable=self.blender_var,
            )
            self.blender_check.pack(anchor="w")

            self.generate_button = tk.Button(root, text="Generate", command=self._on_generate)
            self.generate_button.pack(pady=5)

            self.status = tk.Label(root, text="Idle", anchor="w")
            self.status.pack(fill="x")

        def _on_generate(self):
            prompt = self.prompt_text.get("1.0", "end").strip()
            if not prompt:
                messagebox.showwarning("No prompt", "Please enter a prompt.")
                return

            asset_name = self.name_entry.get().strip() or cfg.default_model_name
            use_blender = self.blender_var.get()

            threading.Thread(
                target=self._generate_worker,
                args=(prompt, asset_name, use_blender),
                daemon=True,
            ).start()

        def _generate_worker(self, prompt: str, asset_name: str, use_blender: bool):
            try:
                self._set_status("Generating...")
                generate_single_asset(cfg, prompt, asset_name, use_blender)
                self._set_status(f"Done: {asset_name}")
            except Exception as e:
                self._set_status(f"Error: {e}")

        def _set_status(self, text: str):
            def _update():
                self.status.config(text=text)

            self.root.after(0, _update)

    root = tk.Tk()
    app = UnityAssetGUI(root)
    root.mainloop()


@cli.command("gui")
def cli_gui():
    """Launch the Unity Asset Generator GUI."""
    _run_gui()


# -----------------------------
# Entry point
# -----------------------------


if __name__ == "__main__":
    os.environ['OPENAI_API_KEY'] = str("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")
    cli()