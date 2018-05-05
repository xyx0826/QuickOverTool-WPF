using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static OWorkbench.Properties.Settings;

namespace OWorkbench.Logics
{
    /// <summary>
    /// The class that handles the creation of command line parameters.
    /// </summary>
    class CommandLineFabricator
    {
        /// <summary>
        /// Creates a base command line containing working mode, language selection and other shared parameters
        /// </summary>
        /// <returns>A StringBuilder containing the base command.</returns>
        public static StringBuilder CreateBaseCommand()
        {
            StringBuilder builder = new StringBuilder();

            if (String.IsNullOrEmpty(ListAssetsHandler.GetInstance().IsTabSelected  // Determine which ComboBoxMode to **check**
                ? ListAssetsHandler.GetInstance().ComboBoxMode.Value                // with an inline if-else (not actually appending yet)
                : ExtrAssetsHandler.GetInstance().ComboBoxMode.Value))
                throw new ArgumentException("Please select an Extract Assets mode to continue.");

            builder.Append("--language=" + DataToolConfig.GetInstance().ComboBoxLanguage.Content + " ");   // Language

            return builder;
        }

        /// <summary>
        /// Appends flags used by extract mode.
        /// </summary>
        /// <param name="builder">A StringBuilder, typically returned from previous command line fabricator methods.</param>
        /// <returns></returns>
        public static StringBuilder AppendExtractFlags(StringBuilder builder)
        {
            ExtrAssetsHandler handler = ExtrAssetsHandler.GetInstance();

            if (!handler.noExtTextures) // if texture will be extracted
            {
                if (!handler.noConTextures) // if texture will be converted
                {
                    if (!handler.isLosslessTexture) builder.Append("--convert-lossless-textures=false ");    // texture lossless
                    builder.Append("--convert-textures-type=" + handler.comboBoxFormat.Content + " ");   // texture format
                }
                else builder.Append("--convert-textures=false ");
            }
            else builder.Append("--skip-textures=true ");

            if (!handler.noExtSound)    // if sound will be extracted
            {
                if (handler.noConSound) builder.Append("--convert-sound=false ");
            }
            else builder.Append("--skip-sound=true ");

            if (!handler.noExtModels)   // if models will be extracted
            {
                builder.Append("--lod=" + handler.modelLOD + " ");   // model LOD
                if (handler.noConModels) builder.Append("--convert-models=false ");
            }
            else builder.Append("--skip-models=true ");

            if (!handler.noExtAnimation)   // if models will be extracted
            {
                if (handler.noConAnimation) builder.Append("--convert-animations=false ");
            }
            else builder.Append("--skip-animations=true ");

            if (!handler.noExtRefpose)  // if refposes will be extracted
                builder.Append("--extract-refpose=true ");

            if (handler.noConAnything) builder.Append("--raw "); // convert nothing

            if (handler.ComboBoxMode.Value == "extract-map-envs")   // if map env extraction is active
            {
                if (handler.noEnvSound) builder.Append("--skip-map-env-sound=true ");
                if (handler.noEnvLUT) builder.Append("--skip-map-env-lut=true ");
                if (handler.noEnvBlend) builder.Append("--skip-map-env-blend=true ");
                if (handler.noEnvCubeMap) builder.Append("--skip-map-env-cubemap=true ");
                if (handler.noEnvGround) builder.Append("--skip-map-env-ground=true ");
                if (handler.noEnvSky) builder.Append("--skip-map-env-sky=true ");
                if (handler.noEnvSkybox) builder.Append("--skip-map-env-skybox=true ");
                if (handler.noEnvEntity) builder.Append("--skip-map-env-entity=true ");
            }

            return builder;
        }

        /// <summary>
        /// Validate and append extract queries based on mode selection.
        /// </summary>
        /// <param name="builder">A StringBuilder, typically returned from previous command line fabricator methods.</param>
        /// <returns></returns>
        public static StringBuilder AppendExtractQueries(StringBuilder builder)
        {
            ExtrAssetsHandler handler = ExtrAssetsHandler.GetInstance();
            if (!handler.IsTabSelected) return builder; // Query is not needed in that case
            
            switch (handler.ComboBoxMode.Value) // Mode-specified queries
            {
                // Leading and trailing quotes shall only be added when there is query, so they had to be in individual switches
                case "extract-unlocks":
                    builder.Append("\"");
                    if (!(String.IsNullOrEmpty(handler.queryUnlockable_Name)    // Verify hero name and type
                        || String.IsNullOrEmpty(handler.queryUnlockable_Type.Value)))
                    {
                        builder.Append(handler.queryUnlockable_Name);
                        builder.Append("|");
                        builder.Append(handler.queryUnlockable_Type.Value);
                    }
                    else throw new ArgumentException("Extract-unlocks mode: invalid queries. Please fill in hero name and type.");

                    if (!String.IsNullOrEmpty(handler.queryUnlockable_Tag.Value))   // Tag is specified
                    {
                        if (String.IsNullOrEmpty(handler.queryUnlockable_Param))
                            throw new ArgumentException("Extract-unlocks mode: invalid queries. Either specify a parameter or unspecify the tag.");

                        builder.Append("=(");
                        builder.Append(handler.queryUnlockable_Tag.Value);
                        builder.Append("=");
                        builder.Append(handler.queryUnlockable_Param);
                        builder.Append(")");
                    }
                    builder.Append("\"");
                    break;

                case "extract-hero-voice":
                    if (!String.IsNullOrEmpty(handler.queryVoice_Name)) // Hero name is specified
                    {
                        builder.Append("\"");
                        builder.Append(handler.queryVoice_Name);

                        if (!String.IsNullOrEmpty(handler.queryVoice_Type.Value))   // Type is specified
                        {
                            if (String.IsNullOrEmpty(handler.queryVoice_Param))
                                throw new ArgumentException("Extract-hero-voice mode: invalid queries. Either specify a parameter or unspecify the type.");

                            builder.Append("|");
                            builder.Append(handler.queryVoice_Type.Value);
                            builder.Append(handler.queryVoice_Param);
                        }
                        builder.Append("\"");
                    }
                    else throw new ArgumentException("Extract-hero-voice mode: invalid queries. Specify a hero name.");
                    break;

                case "extract-npcs":
                    if (!String.IsNullOrEmpty(handler.queryNpc_Name))
                    {
                        builder.Append("\"");
                        builder.Append(handler.queryNpc_Name);
                        builder.Append("\"");
                    }
                    break;

                case "extract-maps":
                    if (!String.IsNullOrEmpty(handler.queryMap_Name))
                    {
                        builder.Append("\"");
                        builder.Append(handler.queryMap_Name);
                        builder.Append("\"");
                    }
                    else throw new ArgumentException("Extract-maps mode: invalid queries. Specify a map name.");
                    break;
            }
            return builder;
        }

        /// <summary>
        /// Appends the paths as the last part of the command line.
        /// </summary>
        /// <param name="builder">A StringBuilder, typically returned from previous command line fabricator methods.</param>
        /// <returns></returns>
        
        public static StringBuilder AppendPathAndMode(StringBuilder builder)
        {
            if (ListAssetsHandler.GetInstance().IsTabSelected   // ListAssets JSON output
                && Default.TAB_LIST_OutputJSON)
                builder.Append(" --json");

            builder.Append(" \"" + UIString.GetInstance().CurrentOWPath + "\" ");
            
            builder.Append((ExtrAssetsHandler.GetInstance().IsTabSelected   // Determine which ComboBoxMode to **use**
                ? ExtrAssetsHandler.GetInstance().ComboBoxMode.Value        // with an inline if-else
                : ListAssetsHandler.GetInstance().ComboBoxMode.Value));     // Value should already be ensured non-null
            
            if (ExtrAssetsHandler.GetInstance().IsTabSelected)  // Extract mode requires a global output path
                builder.Append(" \"" + Default.TAB_SETTINGS_OutputPath + "\" ");
            return builder;
        }
    }
}
