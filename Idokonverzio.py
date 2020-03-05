def formatExpr(val: int, unit: str):
    ret = None    
    if val > 0: ret = '{} {}'.format(val, unit)
    if val > 1: ret += 's'
    return ret

def formatSeconds(seconds: int):
    if seconds < 0: return "Seconds should be positive or zero."
    if seconds == 0: return "now"
    
    M = 60
    H = 60 * M
    D = 24 * H
    Y = 356 * D
    
    l = []
    
    y, seconds = divmod(seconds, Y)
    l.append(formatExpr(y, 'year'))
    
    d, seconds = divmod(seconds, D)
    l.append(formatExpr(d, 'day'))
    
    h, seconds = divmod(seconds, H)
    l.append(formatExpr(h, 'hour'))
    
    m, seconds = divmod(seconds, M)
    l.append(formatExpr(m, 'minute'))
    
    l.append(formatExpr(seconds, 'second'))
    
    return ', '.join([v for v in l if v != None])[::-1].replace(
    " ,", " dan ", 1)[::-1]
    
    
formatSeconds(3600 + 15 * 3600 + 15 + 24 * 3600)
