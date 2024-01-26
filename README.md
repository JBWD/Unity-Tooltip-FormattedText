## Tooltip String Designer / Formatter for Unity

### Designers:

Built for designers in mind, with a built in property drawers and realtime property additions. Simplify your process and allow for more information to be used within a tooltip.

![image](https://github.com/JBWD/Unity-Tooltip-FormattedText/assets/35278058/513a4912-9514-4fad-a70b-b4358c83f2d7)


Allows for both indices and named variable access with the click of a toggle

Clear Values button has been added if during programming a designer doesn't know if the list is up to date click the button. This will clear the values and look as though the list is empty, depending on where the initialization is located, modify a value within the object such as clicking UseIndices twice and this will refresh the values.

![image](https://github.com/JBWD/Unity-Tooltip-FormattedText/assets/35278058/70547305-1116-4880-8f99-fa31e706fa00)


#### Indices:

Example: "Deals {0} to {1} {2} damage to the target."

#### Named Variables:

Example: "Deals {minDamage} to {maxDamage} {damageType} damage to the target."

#### Result:

Example:  "Deals 3 to 8 Fire damage to the target."


<br><br>

### Programmers:

Programmers have been provided 4 different ways to add keys to the formatter:
To build the list automatically call ValidationInitialization() in any of: OnValidate(), OnDrawGizmos(), or OnDrawGizmosSelected().

Note: ScriptableObjects will need to use OnValidate()

#### Option 1: Attributes

The [StringKey] attribute allows for any variable within the current class or inherited class to be added during validation.

```
[StringKey]
public int minDamage = 2,
    maxDamage = 5;
[StringKey]
public string damageType = "Fire";


public FormattedString toolTip;
     
private void OnValidate()
{         
    toolTip.ValidationInitialization(this);
}
```

#### Option 2: Enumeration

Allows for an enumeration to be sent to the FormattedString and which will then use the EnumNames to try add new keys. (Not Case Sensative)

```
public int minDamage = 2,
    maxDamage = 5;
public string damageType = "Fire";

public enum FieldKeys
{
    MinDamage,
    MaxDamage,
    DamageType
}

public FormattedString toolTip;
     
private void OnDrawGizmos()
{         
    toolTip.ValidationInitialization<FieldKeys>(this);
}
```


#### Option 3: String []

Allows for an string [] to be sent to the FormattedString and which will then use the strings to try add new keys. (Not Case Sensative)

```
public int minDamage = 2,
    maxDamage = 5;
public string damageType = "Fire";

public FormattedString toolTip;
     
private void OnValidate()
{         
    toolTip.ValidationInitialization(this, new string [] {"minDamage", "mAxDamage", "damageType"});
}
```

#### Option 4: Manually

Allows for each key to be added manually, this provides the best performance as each object is not using reflection. This also allows for other objects to be added and
also allows for custom names to be added. This can provide consistancy if different programmers are on different parts of a project.

```
public Character character;
public int minDamage = 2,
    maxDamage = 5;
public string damageType = "Fire";

public FormattedString toolTip;
     
private void OnDrawGizmosSelected()
{         
    toolTip.RegisterValue("MinimumDamage", minDamage);
    toolTip.RegisterValue("maxDamage", maxDamage);
    toolTip.RegisterValue("DType", damageType);
    toolTip.RegisterValue("%T", character.DisplayName);
}
```











