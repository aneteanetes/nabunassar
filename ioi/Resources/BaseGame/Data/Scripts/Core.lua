math.randomseed(os.time());

Core = {
    groupby = function(data, key_selector)
        local grouped = {}
        for k, item in ipairs(data) do
            local key = key_selector(item)
            grouped[key] = grouped[key] or {}
            table.insert(grouped[key], item)
        end
        return grouped
    end
}
Templates={
    Base={},
    Perks={},
    Races={},
    Classes={},
    Enemies={}
}