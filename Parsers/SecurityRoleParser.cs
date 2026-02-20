using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class SecurityRoleParser
{
    public List<SecurityRoleDefinition> ParseSecurityRoles(XDocument customizationsXml)
    {
        var roles = new List<SecurityRoleDefinition>();
        
        var roleNodes = customizationsXml.Descendants("Role");
        
        foreach (var roleNode in roleNodes)
        {
            try
            {
                var role = ParseSecurityRole(roleNode);
                roles.Add(role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to parse security role: {ex.Message}");
            }
        }
        
        return roles;
    }
    
    private SecurityRoleDefinition ParseSecurityRole(XElement roleNode)
    {
        var role = new SecurityRoleDefinition
        {
            RoleId = roleNode.Element("roleid")?.Value ?? Guid.NewGuid().ToString(),
            Name = roleNode.Element("name")?.Value ?? "Unknown",
            OriginalXml = roleNode
        };
        
        // Parse privileges
        var privileges = new Dictionary<string, Dictionary<string, string>>();
        var privilegeNodes = roleNode.Element("RolePrivileges")?.Elements("RolePrivilege");
        
        if (privilegeNodes != null)
        {
            foreach (var privNode in privilegeNodes)
            {
                var entityName = privNode.Attribute("name")?.Value;
                var privilegeType = privNode.Attribute("privilegeType")?.Value;
                var depth = GetPrivilegeDepth(privNode.Attribute("depth")?.Value);
                
                if (!string.IsNullOrEmpty(entityName) && !string.IsNullOrEmpty(privilegeType))
                {
                    if (!privileges.ContainsKey(entityName))
                    {
                        privileges[entityName] = new Dictionary<string, string>();
                    }
                    
                    privileges[entityName][privilegeType] = depth;
                }
            }
        }
        
        if (privileges.Any())
        {
            role.Privileges = privileges;
        }
        
        return role;
    }
    
    private string GetPrivilegeDepth(string? depthValue)
    {
        return depthValue switch
        {
            "0" => "None",
            "1" => "User",
            "2" => "Business Unit",
            "3" => "Parent: Child Business Units",
            "4" => "Organization",
            _ => depthValue ?? "Unknown"
        };
    }
}
