using System;
using System.Collections.Generic;

[Serializable]
public class ScenarioData
{
    public string scenario_id;
    public string title;
    public HotspotsData hotspots;
    public EHRConfig ehr_config;
    public RulesData rules;
    public LoggingData logging;
}

[Serializable]
public class HotspotsData
{
    public HotspotInfo hs_monitor;
    public HotspotInfo hs_patient;
    public HotspotInfo hs_ventilator;
    public HotspotInfo hs_ehr;
    public HotspotInfo hs_call;
}

[Serializable]
public class HotspotInfo
{
    public string id;
    public string label;
}

[Serializable]
public class EHRConfig
{
    public FormsConfig forms;
}

[Serializable]
public class FormsConfig
{
    public FormConfig assessment_form;
    public FormConfig intervention_form;
    public FormConfig communication_log;
}

[Serializable]
public class FormConfig
{
    public string title;
    public List<string> fields;
}

[Serializable]
public class RulesData
{
    public List<GlobalRule> global_rules;
    public List<NodeData> nodes;
}

[Serializable]
public class GlobalRule
{
    public string id;
    public RuleCondition condition;
    public List<RuleEffect> effects;
}

[Serializable]
public class RuleCondition
{
    // Θα το επεκτείνουμε αργότερα
}

[Serializable]
public class RuleEffect
{
    public string type;
    public string target;
    public string state;
    public string style;
    public string message;
}

[Serializable]
public class NodeData
{
    public string id;
    public string type; // message, decision, gate, end
    public string text;
    public string description;
    public string next_node_id;
    
    // Decision
    public List<OptionData> options;
    public TimeoutData timeout;
    
    // Gate
    public GateRequirements gate_requirements;
    public string feedback_blocked;
    public string feedback_success;
    public EffectsOnPass effects_on_pass;
    
    // End
    public DebriefConfig debrief_config;
}

[Serializable]
public class OptionData
{
    public string id;
    public string label;
    public string target_hotspot;
    public OptionEffects effects;
    public string next_node_id;
}

[Serializable]
public class OptionEffects
{
    public int score_delta;
    public StateUpdate state_update;
    public VitalsUpdate vitals_update;
    public string toast;
}

[Serializable]
public class StateUpdate
{
    public bool flags_assessment_complete;
    public bool flags_oxygen_adjusted;
    public bool flags_escalation_complete;
    public bool flags_documentation_1_complete;
    public bool flags_documentation_2_complete;
}

[Serializable]
public class VitalsUpdate
{
    public int spo2;
    public int hr;
}

[Serializable]
public class TimeoutData
{
    public int seconds;
    public TimeoutEffects on_timeout_effects;
    public string next_node_id;
}

[Serializable]
public class TimeoutEffects
{
    public VitalsUpdate vitals_update;
    public int score_delta;
    public string toast;
}

[Serializable]
public class GateRequirements
{
    public string target_hotspot;
    public List<RequiredForm> required_forms;
}

[Serializable]
public class RequiredForm
{
    public string form_id;
    public List<string> fields;
}

[Serializable]
public class EffectsOnPass
{
    public StateUpdate state_update;
    public int score_delta;
}

[Serializable]
public class DebriefConfig
{
    public bool show_score;
    public bool show_decision_path;
    public bool highlight_missed_docs;
    public bool export_log;
}

[Serializable]
public class LoggingData
{
    public bool enabled;
    public List<string> log_events;
    public string export_format;
}