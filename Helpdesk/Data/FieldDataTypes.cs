namespace Helpdesk.Data
{
    public enum FieldDataTypes
    {
        // unknown
        NotDefined = 0,
        // check box slider for yes/no
        Boolean = 1,
        // whole number entry
        Integer = 2,
        // decimal number entry
        Number = 3,
        // text entry box
        String = 4,
        // date picker
        Date = 5,
        // date and time picker
        DateTime = 6,
        // time picker
        Time = 7,
        // Entity drop down selector. ReferenceTargetTypes determines the kind of entity this references.
        // The ID of the entity is stored in the data table for this field.
        EntitySelction,
        // References a field on an entity. Value is stored on the entity. 
        // An EntitySelection field is required to specify the target entity that this field references.
        EntityFieldEditor
    }
}
