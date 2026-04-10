using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Netcode;

namespace CounterHeads.Config.Value;

public class WeaponMap : INetworkSerializable
{
    private Weapon[] _weapons;

    public WeaponMap(IEnumerable<Weapon> weapons)
    {
        _weapons = weapons.ToArray();
    }

    public WeaponMap()
    {
        // for network serializable
        _weapons = [];
    }

    public static WeaponMap ParseWeaponConfig(string configString)
    {
        const string colon = ":";
        const string colonEscape = "\\:";
        const string colonEscapeReplacement = "$_colonesc_$";
        const string comma = ",";
        const string commaEscape = "\\,";
        const string commaEscapeReplacement = "$_commaesc_$";
        const string weaponsPattern = @"(?:\{'([^']*)'):([0-9]*)}|(?:\{'([^']*)'})";
        
        string escapeReplacedString = configString.Replace(colonEscape, colonEscapeReplacement)
            .Replace(commaEscape, commaEscapeReplacement);

        List<Weapon> weapons = [];
        
        Match regexMatch = Regex.Match(escapeReplacedString, weaponsPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(2));
        if (!regexMatch.Success)
        {
            CounterHeads.Logger.LogWarning($"No valid values to parse from weapon map, {configString}");
            return new WeaponMap([]);
        }
        while (regexMatch.Success)
        {
            if (regexMatch.Groups[3].Success)
            {
                string weaponName = regexMatch.Groups[3].Value;
                weaponName = weaponName.Replace(commaEscapeReplacement, comma).Replace(colonEscapeReplacement, colon);
                if(CheckIfWeaponAlreadyPresent(weapons, weaponName)) 
                {
                    CounterHeads.Logger.LogWarning($"Weapon '{weaponName}' was listed multiple times in config, only the first entry will be used");
                    regexMatch = regexMatch.NextMatch();
                    continue;
                }
                weapons.Add(new Weapon(regexMatch.Groups[3].Value, 0));
                CounterHeads.Instance.LogInfoIfExtendedLogging($"Found weapon with no damage at {regexMatch.Groups[3].Index}. Group 3 value: {weaponName}, Match value: {regexMatch.Value}");
            } 
            else if (regexMatch.Groups[1].Success && regexMatch.Groups[2].Success)
            {
                string weaponName = regexMatch.Groups[1].Value;
                weaponName = weaponName.Replace(commaEscapeReplacement, comma).Replace(colonEscapeReplacement, colon);
                if(CheckIfWeaponAlreadyPresent(weapons, weaponName)) 
                {
                    CounterHeads.Logger.LogWarning($"Weapon '{weaponName}' was listed multiple times in config, only the first entry will be used");
                    regexMatch = regexMatch.NextMatch();
                    continue;
                }
                weapons.Add(new Weapon(weaponName, int.Parse(regexMatch.Groups[2].Value, NumberStyles.Integer)));
                CounterHeads.Instance.LogInfoIfExtendedLogging($"Found weapon with damage at {regexMatch.Groups[1].Index} and {regexMatch.Groups[2].Index}. Group 1 value: {weaponName}, Group 2 value: {regexMatch.Groups[2].Value}, Match value: {regexMatch.Value}");
            }
            else
            {
                CounterHeads.Logger.LogWarning("No groups matched");
            }
            regexMatch = regexMatch.NextMatch();
        }

        return new WeaponMap(weapons);
    }

    private static bool CheckIfWeaponAlreadyPresent(List<Weapon> weapons, string name)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.NameMatches(name.ToLowerInvariant()))
            {
                return true;
            }
        }

        return false;
    }

    public Weapon? TryGetWeaponData(string name)
    {
        foreach (var weapon in _weapons)
        {
            if (weapon.NameMatches(name.ToLowerInvariant()))
            {
                return weapon;
            }
        }

        return null;
    }
    
    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        int length = 0;
        Weapon[] tempWeapons = new Weapon[length];
        if (serializer.IsWriter)
        {
            tempWeapons = _weapons;
            length = tempWeapons.Length;
        }
        
        serializer.SerializeValue(ref length);
        
        if (!serializer.IsWriter)
        {
            tempWeapons = new Weapon[length];
        }

        for (int n = 0; n < length; ++n)
        {
            serializer.SerializeValue(ref tempWeapons[n]);
        }

        if (serializer.IsReader)
        {
            _weapons = tempWeapons;
        }
    }

    public struct Weapon : INetworkSerializable
    {
        private string _name;
        private int _damage;

        internal Weapon(string name, int damage)
        {
            _name = name.ToLowerInvariant();
            _damage = damage;
        }

        public int GetDamage()
        {
            return _damage;
        }

        internal bool NameMatches(string weaponNameLower)
        {
            return _name.Equals(weaponNameLower);
        }
        
        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> bufferSerializer)
        {
            bufferSerializer.SerializeValue(ref _name);
            bufferSerializer.SerializeValue(ref _damage);
        }
    }
}